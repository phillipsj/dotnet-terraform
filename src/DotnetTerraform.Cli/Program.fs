// Learn more about F# at http://fsharp.org

open System
open DotnetTerraform.Cli.Commands
open Argu

type CliError =
    | ArgumentsNotSpecified
    
let getExitCode result =
    match result with
    | Ok () -> 0
    | Error err ->
        match err with
        | ArgumentsNotSpecified -> 1
        
[<EntryPoint>]
let main argv =
    let errorHandler = ProcessExiter(colorizer = function ErrorCode.HelpText -> None | _ -> Some ConsoleColor.Red)
    let parser = ArgumentParser.Create<CmdArgs>(programName = "dotnet-terraform", errorHandler = errorHandler)
    
    
    match parser.ParseCommandLine argv with
    | p when p.Contains(Install) ->
        printfn "Install command"
        Ok()
    | p when p.Contains(Init) ->
        p.GetResults(Init).ToString() |> printfn "Init command with the following %s" 
        Ok()
    | _ ->
        parser.PrintUsage() |> printfn "%s" 
        Error ArgumentsNotSpecified
    |> getExitCode
