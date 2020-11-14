namespace ApplicationLogicLibrary.Controller

open System
open System.Threading
open ApplicationLogicLibrary.Model
open ApplicationLogicLibrary.Controller
open ApplicationLogicLibrary.Controller.CellController


module ConnectionController = 
    
    let CreateNewCell2 move cell  = 
        match GetNewPosition move cell.Position 1 with
        | Valid when move <> Stay -> createCell [ move ] (GetNewPosition move cell.Position 1) (int NamedColor.Green) move
        | NotValid -> createCell [ move ] {X=99;Y=99} (int NamedColor.Green) move
    
    let CreateNewCell1 cell move = 
        match GetNewPosition move cell.Position 1 with
        | Valid when move <> Stay -> createCell [ move ] (GetNewPosition move cell.Position 1) (int NamedColor.White) move
        | NotValid when ((GetNewPosition Left (GetNewPosition Down cell.Position 1) 1) |> Valid) -> 
            createCell [ Left ] (GetNewPosition Down cell.Position 1) (int NamedColor.White) Down
        | NotValid when ((GetNewPosition Right (GetNewPosition Down cell.Position 1) 1) |> Valid) -> 
            createCell [ Right ] (GetNewPosition Down cell.Position 1) (int NamedColor.White) Down

    let ExecuteFigures1 figures =
        let firstFigure = List.head figures
        let lastCell = List.last firstFigure
        [firstFigure @ [CreateNewCell1 lastCell (List.head lastCell.PossibleMoves)]]

    let ExecuteCells behavior cell = cell.PossibleMoves |> List.map (behavior cell)

    let ExecuteFigures2 figures =
        let lastFigure = List.last figures
        let lastCell = List.last lastFigure
        let newFigure = ((lastFigure |> List.map (CreateNewCell2 Up)) @ [CreateNewCell2 Right lastCell]) |> List.filter (fun cell -> cell.Position.X <> 99)  //mit List.choose schoener
        figures @ [newFigure]
    
    let testCellFn1 figures = [ (List.last figures) |> List.last ]
    let testCellFn2 figures = List.last figures

    let EndCondition testCellFn figures =
        let endTestCells = testCellFn figures
        if List.isEmpty endTestCells then true
        else
            (List.exists (fun move -> 
                (endTestCells
                |> List.map CellPosition
                |> List.exists (fun position -> (GetNewPosition move position 1 |> IsValidPosition (List.last figures)))))
                [ Right; Down; Left; Up ]) |> not //railway possible

    let Connected() =
        (Events.LibraryEvents.Instance.SendArduinoCommand
             (Command.ArduinoCommand
                  (Command.WithoutParameter
                       (CommandName.Connected))))
        let workflow = 
            async { 
                do! Modi [[ createCell [ Right ] { X = 0; Y = 0 } (int NamedColor.White) Stay ]] ExecuteFigures1 50 (EndCondition testCellFn1)
                do! Modi [[ createCell [ Up; Right ] { X = 0; Y = 5 } (int NamedColor.Green) Stay ]] ExecuteFigures2 50 (EndCondition testCellFn2)
            }
        Async.Start workflow
