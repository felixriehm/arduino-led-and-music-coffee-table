namespace ApplicationLogicLibrary.Model

open System

module Table = 
    let Rows = 6
    let Columns = 8
    
    let create columns rows tiles tracks games modis = 
        { Columns = columns
          Rows = rows
          Tiles = tiles
          Tracks = tracks
          Games = games
          Modis = modis }
    
    let mutable tiles : seq<Tile> = 
        Seq.cast (Array2D.init 6 8 (fun x y -> 
                      { X = x
                        Y = y
                        Hue = 180 }))
    
    let (|Even|Odd|) n =
        if n % 2 = 0 then Even
        else Odd

    let tilesTansformToViewpointRight row column =
        match row with
        | Odd -> (row * Columns + column) * 3
        | Even -> (row * Columns + Columns - 1 - column) * 3
    
    let tilesTansformToViewpointUp row column =
        match column with
        | Odd -> (column * Columns + Columns - 1 - row) * 3
        | Even -> (column * Columns + row) * 3
    
    let tilesTansformToViewpointDown row column =
        match column with
        | Odd -> ((48 - 1) - (column * Columns + row)) * 3
        | Even -> ((48 - 1) - (column * Columns + Columns - 1 - row)) * 3
    
    let tilesTansformToViewpointLeft row column =
        match row with
        | Odd -> ((48 - 1) - (row * Columns + Columns - 1 - column)) * 3
        | Even -> ((48 - 1) - (row * Columns + column)) * 3
    
    let tilesToString viewTransformation tiles = 
        tiles
        |> Seq.sortBy (fun tile -> viewTransformation tile.X tile.Y)
        |> Seq.fold (fun str n -> str + n.Hue.ToString("D3")) ""

    let tileToString (viewTransformation: int->int->int, x:int, y:int, hue:int)= 
        [(viewTransformation x y).ToString("D3") + hue.ToString("D3")]
    
    let setTile (column : int, row : int, hue : int) = 
        (Seq.find (fun x -> x.X = column && x.Y = row) tiles).Hue <- hue
        let test = tileToString (tilesTansformToViewpointLeft,column,row,hue)
        (Events.LibraryEvents.Instance.SendArduinoCommand
             (Command.ArduinoCommand
                  (Command.WithParameter
                       (CommandName.SetTile, test))))
        Events.LibraryEvents.Instance.UpdateUiTile column row hue
    
    let setAllTiles (updatedTiles : Tile [,]) = 
        tiles <- (Seq.cast updatedTiles)
        (Events.LibraryEvents.Instance.SendArduinoCommand
             (Command.ArduinoCommand
                  (Command.WithParameter
                       (CommandName.SetAllTiles, [ tilesToString tilesTansformToViewpointLeft tiles ]))))
        Events.LibraryEvents.Instance.UpdateAllUiTiles tiles
    
    let setAllTilesToColor (hue : int) = 
        Seq.iter (fun tile -> tile.Hue <- hue) tiles
        (Events.LibraryEvents.Instance.SendArduinoCommand
             (Command.ArduinoCommand
                  (Command.WithParameter
                       (CommandName.SetAllTiles, [ tilesToString tilesTansformToViewpointLeft tiles ]))))
        Events.LibraryEvents.Instance.UpdateAllUiTiles tiles
