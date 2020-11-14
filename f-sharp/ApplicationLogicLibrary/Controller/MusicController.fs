namespace ApplicationLogicLibrary.Controller

open System
open ApplicationLogicLibrary.Model

module MusicController = 
    let tracks : seq<Track> = 
        (Seq.cast [ { Title = "Get Lucky"
                      StorageName = "gl.afm"
                      Duration = new TimeSpan(0, 10, 0) }
                    { Title = "What is Love"
                      StorageName = "wil.afm"
                      Duration = new TimeSpan(0, 4, 16) }
                    { Title = "Cantina Theme"
                      StorageName = "ct.afm"
                      Duration = new TimeSpan(0, 2, 09) } ])
    
    let playMusic track = 
        (Events.LibraryEvents.Instance.SendArduinoCommand
             (Command.ArduinoCommand
                  (Command.WithParameter(CommandName.PlayMusic, [ track.StorageName ]))))
    let stopMusic() = 
        (Events.LibraryEvents.Instance.SendArduinoCommand
             (Command.ArduinoCommand(Command.WithoutParameter(CommandName.StopMusic))))
    let pauseMusic() = 
        (Events.LibraryEvents.Instance.SendArduinoCommand
             (Command.ArduinoCommand(Command.WithoutParameter(CommandName.PauseMusic))))
