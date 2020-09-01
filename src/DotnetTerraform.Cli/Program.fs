// Learn more about F# at http://fsharp.org

open System
open Argu

type CliArguments =
    | Install
    | Fmt of args: string
    | Apply of args: string

[<EntryPoint>]
let main argv =
    printfn "Hello World from F#!"
    0 // return an integer exit code
