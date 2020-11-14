namespace ApplicationLogicLibrary.Model

open System
open System.Collections.Generic

[<AutoOpen>]
module ModelTypes = 
    type CommandName = 
        | Connected = 1
        | SetTile = 2
        | SetAllTiles = 3
        | PlayMusic = 4
        | StopMusic = 5
        | PauseMusic = 6
        | PrintFreeRam = 7

    type NamedColor =
        | Red = 0
        | Yellow = 60
        | Green = 120
        | Cyan = 180
        | Blue = 240
        | Magenta = 300
        | Black = 370
        | White = 371
    
    type Tile = 
        { mutable X : int
          mutable Y : int
          mutable Hue : int }
    
    type Track = 
        { Title : string
          StorageName : string
          Duration : TimeSpan }
    
    type Procedure = Async<unit>
    
    type Modi = 
        { Name : string
          Procedure : Procedure }
    
    type Game = 
        { Name : string
          Procedure : Procedure }
    
    type Table = 
        { Columns : int
          Rows : int
          mutable Tiles : IEnumerable<Tile>
          mutable Tracks : IEnumerable<Track>
          mutable Games : IEnumerable<Game>
          mutable Modis : IEnumerable<Modi> }
