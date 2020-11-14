namespace ApplicationLogicLibrary.Controller

open System
open System.Threading
open ApplicationLogicLibrary.Model
open ApplicationLogicLibrary.Controller
open ApplicationLogicLibrary.Controller.CellController

module ModiController = 
    let mutable cancellationSource = new CancellationTokenSource()
    
    let startModi (modi : Modi) = 
        cancellationSource <- new CancellationTokenSource()
        Async.Start(modi.Procedure, cancellationSource.Token)
    
    let stopModi() = cancellationSource.Cancel()
    
    let randomHsv =
        let rndGen = System.Random()
        let randomHsvCreateCell cell = 
            createCell [ ] cell.Position (rndGen.Next(0, 360)) Stay
    
        let executeFigures figures=
            let firstFigure = List.head figures
            [firstFigure |> List.map randomHsvCreateCell]

        let endCondition figures = false

        async { 
            do! Modi [[ for y in [ 0..5 ] do for x in [ 0..7 ] do yield createCell [ ] { X = x; Y = y } (rndGen.Next(0, 360)) Stay ]] executeFigures 2000 endCondition
        }
    
    let transitionHsv =
        let clampValue hue =
            if hue > 359 then 0
            else hue

        let rndGen = System.Random()
        let transitionHsvCreateCell cell = 
            createCell [ ] cell.Position (clampValue (cell.Color+2)) Stay
        
        let executeFigures figures=
            let firstFigure = List.head figures
            [firstFigure |> List.map transitionHsvCreateCell]

        let endCondition figures = false

        async { 
            do! Modi [[ for y in [ 0..5 ] do for x in [ 0..7 ] do yield createCell [ ] { X = x; Y = y } (rndGen.Next(0, 360)) Stay ]] executeFigures 20 endCondition
        }


    let pipes =
        let pipeMaxSize = 20
        let singlePipeMinLen = 2;
        let rndGen = System.Random()
        let possibleMoves = [Right;Left;Down;Up]
        let possibleColors = [int NamedColor.Red;int NamedColor.Yellow;int NamedColor.Green;int NamedColor.Blue]
        let changeDirection() = rndGen.Next(0, 101) > 50

        let randomItem possibleItems lastUsedItem =
            let possibleItemsSize = List.length possibleItems
            let randomItemIndex = rndGen.Next(0, possibleItemsSize)
            let newItem = possibleItems.Item(randomItemIndex)
            if(newItem = lastUsedItem) then possibleItems.Item(Modulo (randomItemIndex+1) possibleItemsSize) else newItem

        let randomMove lastMove =
            if(lastMove = Right || lastMove = Left) then
                if (rndGen.Next(0, 2) = 1) then Up else Down
            else
                if (rndGen.Next(0, 2) = 1) then Left else Right

        let pipesCreateCell cell move = 
            match move with
            | _ -> createCell [ move ] ((GetNewPosition move cell.Position 1) |> CycleThroughWall 8 6) cell.Color move
    
        let endCondition figures = false
            
        let trimPipe figures =
            if(List.sumBy List.length figures > pipeMaxSize) then
                let oldestFigure = List.head figures
                let trimmedPipe = List.tail oldestFigure
                if(List.isEmpty trimmedPipe) then
                    List.tail figures
                else
                    [trimmedPipe] @ List.tail figures
            else
                figures

        let executeFigures figures =
            let lastFigure = List.last figures
            let lastCell = List.last lastFigure
            if( List.length lastFigure > singlePipeMinLen && changeDirection()) then
                let newMove = randomMove lastCell.LastMove
                figures @ [[createCell [ newMove] ((GetNewPosition newMove lastCell.Position 1)|>CycleThroughWall 8 6) (randomItem possibleColors lastCell.Color) lastCell.LastMove]]
            else
                (figures |> List.rev |> List.tail |> List.rev) @ [lastFigure @ [pipesCreateCell lastCell (List.head lastCell.PossibleMoves)]]
            |> trimPipe

        async { 
            do! Modi [[ createCell [ randomItem possibleMoves Stay] { X = rndGen.Next(0, 8); Y = rndGen.Next(0, 6) } (randomItem possibleColors 0) Stay ]] executeFigures 200 endCondition
        }
    let stripes =
        let possibleColors = [int NamedColor.Red;int NamedColor.Yellow;int NamedColor.Green;int NamedColor.Blue]

        let stripesCreateCell move color cell = 
            match move with
            | _ -> createCell [ ] ((GetNewPosition move cell.Position 1) |> CycleThroughWall 9 6) color move
    
        let endCondition figures = false

        let executeFigures (figures : Cell list list) =
            let changedDirection() = System.Random().Next(0,101)

            let changeCondition = changedDirection() > 90 && List.item 0 figures |> List.head |> (fun cell -> cell.Position.Y % 5 = 1)

            let changeMove move = if(changeCondition) then invertMove move else move
               
            let changeColor lastColor= if(changeCondition) then List.item (Modulo 4 (((List.findIndex (fun color -> lastColor = color)) possibleColors)+1)) possibleColors else lastColor

            let longStripes =
                [for i in [0 .. 2] do
                    let cellFromStripe = (List.item i figures) |> List.head
                    let move = changeMove cellFromStripe.LastMove
                    let color = changeColor cellFromStripe.Color
                    yield List.item i figures |> List.map (stripesCreateCell move color )]

            let shortStripes =
                let cellFromStripe = (List.item 3 figures) |> List.head
                let move = changeMove cellFromStripe.LastMove
                let color = changeColor cellFromStripe.Color

                if(List.item 0 figures |> List.head |> (fun cell -> cell.Position.Y % 5 = 1)) then
                    [for i in [3 .. 5] do yield List.item i figures |> List.map (stripesCreateCell move color )]
                else
                    [for i in [3 .. 5] do yield List.item i figures]

            longStripes @ shortStripes

        let longStripes = [ for y in [ 0..2 ] do yield [for x in [ 0..7 ] do yield createCell [ ] { X = x; Y = y*2 } (possibleColors.[y]) Up ]]

        let shortStripes = [ for y in [ 0..2 ] do yield [for x in [ 0..5 ] do yield createCell [ ] { X = y*3; Y = x } (possibleColors.[3]) Right ]]

        async { 
            do! Modi (longStripes @ shortStripes) executeFigures 100 endCondition
        }
    
    let modis : seq<Modi> = 
        Seq.cast [ { Modi.Name = "Random Hsv"
                     Procedure = randomHsv }
                   { Name = "Verlauf Hsv"
                     Procedure = transitionHsv }
                   { Name = "Pipes"
                     Procedure = pipes }
                   { Name = "Stripes"
                     Procedure = stripes } ]
