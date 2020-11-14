namespace ApplicationLogicLibrary.Controller

open System
open ApplicationLogicLibrary.Model
open ApplicationLogicLibrary.Controller

module CellController = 
    let createCell possibleMoves position color lastMove = 
        { PossibleMoves = possibleMoves
          Position = position
          Color = color
          LastMove = lastMove }
    
    let Valid position = position.X >= 0 && position.X < 8 && position.Y >= 0 && position.Y < 6
    
    let Modulo b a = 
        let r = a % b
        if (r < 0) then r + b
        else r
    
    let CycleThroughWall xRange yRange position=
        { X = position.X |> Modulo xRange
          Y = position.Y |> Modulo yRange}
        

    let (|Valid|NotValid|) position = 
        if position.X >= 0 && position.X < 8 && position.Y >= 0 && position.Y < 6 then Valid
        else NotValid
    
    let GetNewPosition move position step= 
        match move with
        | Right -> 
            { X = position.X + step
              Y = position.Y }
        | Left -> 
            { X = position.X - step
              Y = position.Y }
        | Down -> 
            { X = position.X
              Y = position.Y + step }
        | Up -> 
            { X = position.X
              Y = position.Y - step }
        | Stay -> 
            { X = position.X
              Y = position.Y }
        | Jump(x, y) -> 
            { X = x
              Y = y }

    let invertMove move =
                match move with
                | Up -> Down
                | Down -> Up
                | Right -> Left
                | Left -> Right

    let NotTaken figure position = 
        match List.tryFind (fun cell -> cell.Position.X = position.X && cell.Position.Y = position.Y) figure with
        | Some i -> false
        | None -> true

    let debugTile cell =
        System.Diagnostics.Debug.WriteLine("X: " + cell.Position.X.ToString() + " Y: " + cell.Position.Y.ToString() + " C: " + cell.Color.ToString())
        cell
    
    let debugFigure figure =
        System.Diagnostics.Debug.WriteLine "-------FIGURE---------"
        for tile in figure do
            debugTile tile |> ignore
        figure

    let debugFigures figures = 
        System.Diagnostics.Debug.WriteLine "-------NEW DRAWING---------"
        for figure in figures do
            debugFigure figure |> ignore
        figures
    
    let IsValidPosition figure position = Valid position && NotTaken figure position
    let CellPosition cell = cell.Position
    let CellPossibleMoves cell = cell.PossibleMoves
    
    let drawFigures figures = 
        let background = 
            Array2D.init 6 8 (fun x y -> 
                { X = x
                  Y = y
                  Hue = int NamedColor.Black })
        
        let drawFigureOnBack background figure = 
            Array2D.init 6 8 (fun x y -> 
                match List.tryFind (fun cell -> cell.Position.X = y && cell.Position.Y = x) figure with
                | Some(c) -> 
                    { X = x
                      Y = y
                      Hue = c.Color }
                | None -> 
                    { X = x
                      Y = y
                      Hue = (Array2D.get background x y).Hue })
        
        Table.setAllTiles (List.fold (fun acc elem -> drawFigureOnBack acc elem) background figures)
    
    let rec ModiRec figures executeFiguresFn time endCondition = 
        if (endCondition figures) then async { 
                                           drawFigures figures
                                           //debugFigures figures |> ignore
                                           }
        else 
            async { 
                drawFigures figures
                do! Async.Sleep(time)
                //debugFigures figures |> ignore
                do! ModiRec (executeFiguresFn figures) executeFiguresFn time endCondition
            }
    
    let Modi figures executeFiguresFn time endCondition = 
        async { do! ModiRec figures executeFiguresFn time endCondition }

    let KnuthShuffle (lst : array<'a>) =                   // '
        let Swap i j =                                                  // Standard swap
            let item = lst.[i]
            lst.[i] <- lst.[j]
            lst.[j] <- item
        let rnd = new Random()
        let ln = lst.Length
        [0..(ln - 2)]                                                   // For all indices except the last
        |> Seq.iter (fun i -> Swap i (rnd.Next(i, ln)))                 // swap th item at the index with a random one following it (or itself)
        lst  
