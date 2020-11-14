namespace ApplicationLogicLibrary.Model

open System

module Command = 
    type ArduinoCommandParameter = 
        | WithParameter of CommandName * List<string>
        | WithoutParameter of CommandName
    
    let ArduinoCommand x = 
        match x with
        | WithParameter(commandName, commandParameter) -> 
            ((int) commandName).ToString() + " " + String.concat " " commandParameter + "\r"
        | WithoutParameter commandName -> ((int) commandName).ToString() + "\r"
