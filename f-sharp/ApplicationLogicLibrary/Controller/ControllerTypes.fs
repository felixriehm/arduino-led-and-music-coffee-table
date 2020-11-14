namespace ApplicationLogicLibrary.Controller

open System
open ApplicationLogicLibrary.Model

[<AutoOpen>]
module ControllerTypes = 

    type UserInput = 
        | Up = 1
        | Left = 2
        | Down = 3
        | Right = 4
        | Action = 5

    type Move = 
        | Left
        | Right
        | Down
        | Up
        | Jump of x : int * y : int
        | Stay
    
    type Path = Move list
    
    type Position = 
        { X : int
          Y : int }
    
    type Cell = 
        { PossibleMoves : List<Move>
          Position : Position
          Color : int
          LastMove : Move }
  
    type Figure = List<Cell>
