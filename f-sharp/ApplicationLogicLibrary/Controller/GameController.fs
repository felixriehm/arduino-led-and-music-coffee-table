namespace ApplicationLogicLibrary.Controller

open System
open System.Threading
open ApplicationLogicLibrary.Model
open ApplicationLogicLibrary.Controller
open ApplicationLogicLibrary.Controller.CellController
open ApplicationLogicLibrary.Controller.ControllerTypes

module GameController = 
    let mutable cancellationSource = new CancellationTokenSource()

    let startGame (game : Game) = 
        cancellationSource <- new CancellationTokenSource()
        Async.Start(game.Procedure, cancellationSource.Token)
    
    let stopGame() = cancellationSource.Cancel()
    
    let mutable UserInput: UserInput = UserInput.Right
    let mutable userInputAvailable = false


    let userInputToMove userInput=
        match userInput with
        | UserInput.Up -> Move.Up
        | UserInput.Down -> Move.Down
        | UserInput.Left -> Move.Left
        | UserInput.Right -> Move.Right

    let dontMoveMirrorWise lastMove newMove =
        if(newMove = invertMove lastMove) then
            lastMove
        else
            newMove

    type GameObjects = { Snake : Cell list; Food : Cell list}

    let snake =
        
        let createNewSnakeCell i cell figures =
            let move =
                let indeOfPredecessor = i-1
                if(indeOfPredecessor < 0) then
                    UserInput |> userInputToMove |> dontMoveMirrorWise cell.LastMove
                else
                    let snake = List.head (List.tail figures)
                    let predecessor = List.item indeOfPredecessor snake
                    predecessor.LastMove

            createCell [ ] ((GetNewPosition move cell.Position 1) |> CycleThroughWall 8 6) (int NamedColor.Blue) move

        let placeFood oldPosition snake = 
            let possiblePositions = List.except (List.map (fun cell -> cell.Position) snake@[oldPosition]) [for x in [0..7] do for y in [0..5] do yield {X=x;Y=y}]
            if(List.isEmpty possiblePositions) then
                oldPosition
            else
                List.item (System.Random().Next(0,possiblePositions.Length)) possiblePositions

        let enlargeSnake snake =
            let lastCell = List.last snake
            snake @ [createCell [ ] ((GetNewPosition (invertMove lastCell.LastMove) lastCell.Position 1) |> CycleThroughWall 8 6) (int NamedColor.Blue) lastCell.LastMove]
        
        let executeFigures (figures: Cell list list) =
            let snakeMove gameObjects =
                {Snake= gameObjects.Snake |> List.mapi (fun i cell -> (createNewSnakeCell i cell figures)); Food = gameObjects.Food}

            let snakeEat gameObjects =
                if(gameObjects.Snake.Head.Position = gameObjects.Food.Head.Position) then
                    {Snake= enlargeSnake gameObjects.Snake; Food = [createCell [ ] (placeFood gameObjects.Food.Head.Position gameObjects.Snake) (int NamedColor.Red) Right]}
                else
                    gameObjects

            let gameObjects = {Snake= List.head (List.tail figures); Food= (List.head figures)}

            let changedGameObjects = gameObjects |> snakeMove |> snakeEat

            [changedGameObjects.Food] @ [changedGameObjects.Snake]

        let endCondition figures =
            let snake = List.head (List.tail figures)
            let snakeHead = (List.head snake).Position
            List.exists (fun cell -> cell.Position = snakeHead) (List.tail snake)

        let snake = [[ createCell [ ] { X = 1; Y = 0 } (int NamedColor.Blue) Right; createCell [ ] { X = 0; Y = 0 } (int NamedColor.Blue) Right ]]

        let food = [[ createCell [ ] (placeFood { X = 0; Y = 0 } (List.last snake)) (int NamedColor.Red) Right ]]

        async { 
            do! Modi (food @ snake) executeFigures 500 endCondition
        }

    let memory =
        let mutable cardsColor = [||]
        let create4BlockofCell position color = [createCell [ ] { X = position.X; Y = position.Y } color Right;createCell [ ] { X = position.X+1; Y = position.Y } color Right;createCell [ ] { X = position.X; Y = position.Y+1 } color Right;createCell [ ] { X = position.X+1; Y = position.Y+1 } color Right]

        let positionIsWhichCardColor position =
            match position with
            | {X=0; Y=0} -> cardsColor.[0]
            | {X=0; Y=2} -> cardsColor.[1]
            | {X=0; Y=4} -> cardsColor.[2]
            | {X=2; Y=0} -> cardsColor.[3]
            | {X=2; Y=2} -> cardsColor.[4]
            | {X=2; Y=4} -> cardsColor.[5]
            | {X=4; Y=0} -> cardsColor.[6]
            | {X=4; Y=2} -> cardsColor.[7]
            | {X=4; Y=4} -> cardsColor.[8]
            | {X=6; Y=0} -> cardsColor.[9]
            | {X=6; Y=2} -> cardsColor.[10]
            | {X=6; Y=4} -> cardsColor.[11]
            
        let executeFigures (figures : Cell list list) =
            // figures index -> 0: aufgedeckte karten, 1: cursor, 2: aufgedeckte paare
            let revealedUnPairedCards = List.item 2 figures
            let cursor = List.item 1 figures
            let revealedPairedCards = List.item 0 figures
            if(userInputAvailable) then
                userInputAvailable <- false
                if(UserInput = ControllerTypes.UserInput.Action) then // Decke Karte auf
                    if( List.exists (fun cell -> cell.Position = cursor.Head.Position) revealedUnPairedCards || List.exists (fun cell -> cell.Position = cursor.Head.Position) revealedPairedCards) then // Cursor steht auf einer schon aufgedeckten Paare
                        figures
                    else 
                        // schon aufgedeckte Paare + cursor + erstelle neue sichtbarekarte und verbinde sie mit der einen vorhandenen
                        [revealedPairedCards] @ [cursor] @ [create4BlockofCell cursor.Head.Position (positionIsWhichCardColor cursor.Head.Position) @ revealedUnPairedCards]
                else // Bewege Cursor
                    if((revealedUnPairedCards).Length = 8) then //Es sind zwei Karten aufgedeckt
                        if((revealedUnPairedCards.Item 0).Color = (revealedUnPairedCards.Item 4).Color) then // Zwei gleiche paare
                            // lösche gleiche paare und fuege sie zu den aufgedeckten zu
                            [revealedPairedCards @ revealedUnPairedCards] @ [create4BlockofCell ((GetNewPosition (userInputToMove UserInput) cursor.Head.Position 2)|> (CycleThroughWall 8 6)) (int NamedColor.White)] @ [[]]
                        else // Zwei ungleiche paare
                            // lösche ungleiche paare
                            [revealedPairedCards] @ [create4BlockofCell ((GetNewPosition (userInputToMove UserInput) cursor.Head.Position 2)|> (CycleThroughWall 8 6)) (int NamedColor.White)] @ [[]]
                    else //Es sind keine zwei Karten aufgedeckt
                        // fuege eine Karte oder leere Liste (keine Karte) hinzu + cursor + aufgedeckte Paare
                        [revealedPairedCards] @ [create4BlockofCell ((GetNewPosition (userInputToMove UserInput) cursor.Head.Position 2)|> (CycleThroughWall 8 6)) (int NamedColor.White)] @ [revealedUnPairedCards]
            else
                figures
            

        let endCondition figures = List.length (List.item 0 figures) = 40 && List.length (List.item 2 figures) = 8

        async {
            let possibleColors = [int NamedColor.Red;int NamedColor.Yellow;int NamedColor.Green;int NamedColor.Blue;int NamedColor.Cyan;int NamedColor.Magenta]
            cardsColor <- (List.toArray (possibleColors @ possibleColors)) |> KnuthShuffle
            do! Modi [[];create4BlockofCell {X=0;Y=0} (int NamedColor.White); []] executeFigures 100 endCondition
        }
    
    let games : seq<Game> = 
        Seq.cast [ { Name = "Snake"
                     Procedure = snake }
                   { Name = "Memory"
                     Procedure = memory } ]
