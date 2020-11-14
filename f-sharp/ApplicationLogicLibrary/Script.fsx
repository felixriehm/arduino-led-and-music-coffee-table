// Weitere Informationen zu F# finden Sie unter http://fsharp.org. Im Projekt "F#-Lernprogramm" finden Sie
// einen Leitfaden zum Programmieren in F#.

#load "Model/ModelTypes.fs"
#load "Model/Events.fs"
#load "Model/Command.fs"
#load "Model/Table.fs"

open ApplicationLogicLibrary.Model
open System

let Modulo b a = 
        let r = a % b
        if (r < 0) then r + b else r

8 |> Modulo 8

Modulo 1 8

Modulo 8 1