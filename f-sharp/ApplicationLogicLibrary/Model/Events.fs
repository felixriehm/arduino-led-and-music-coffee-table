namespace ApplicationLogicLibrary.Model

open System
open System.Threading

module Events = 
    open System.Collections.Generic

    type ArduinoCommandEventArgs(command : string) = 
        inherit System.EventArgs()
        member this.Command = command
    
    type TileUpdateEventArgs(x : int, y : int, hue : int) = 
        inherit System.EventArgs()
        member this.X = x
        member this.Y = y
        member this.Hue = hue
    
    type AllTilesUpdateEventArgs(tiles : IEnumerable<Tile>) = 
        inherit System.EventArgs()
        member this.Tiles = tiles
    
    type LibraryEvents private () = 
        let arduinoCommandEvent = new DelegateEvent<EventHandler<ArduinoCommandEventArgs>>()
        let tileUpdateEvent = new DelegateEvent<EventHandler<TileUpdateEventArgs>>()
        let allTilesUpdateEvent = new DelegateEvent<EventHandler<AllTilesUpdateEventArgs>>()
        static let instance = LibraryEvents()
        static member Instance = instance
        
        [<CLIEvent>]
        member this.ArduinoCommandEvent = arduinoCommandEvent.Publish
        
        [<CLIEvent>]
        member this.TileUpdateEvent = tileUpdateEvent.Publish
        
        [<CLIEvent>]
        member this.AllTilesUpdateEvent = allTilesUpdateEvent.Publish
        
        member internal this.SendArduinoCommand command = 
            arduinoCommandEvent.Trigger([| this
                                           new ArduinoCommandEventArgs(command) |])
        
        member internal this.UpdateUiTile x y hue = 
            tileUpdateEvent.Trigger([| this
                                       new TileUpdateEventArgs(x,y,hue) |])
        
        member internal this.UpdateAllUiTiles tiles = 
            allTilesUpdateEvent.Trigger([| this
                                           new AllTilesUpdateEventArgs(tiles) |])
