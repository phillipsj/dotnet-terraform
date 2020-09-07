module DotnetTerraform.Cli.TfCommands

open System
open System.Diagnostics
open System.IO
open System.Text

let tfExe =
    Path.Combine(Environment.CurrentDirectory, "terraform").ToString()

let executeTerraform arguments =
    let tfProcess =
        ProcessStartInfo
            (FileName = tfExe,
             Arguments = "version",
             UseShellExecute = false,
             RedirectStandardOutput = true,
             RedirectStandardError = true)
        |> Process.Start
    let sb = StringBuilder()
    let flushContents =
        let flushStream (stream:StreamReader) =
            while not stream.EndOfStream do sb.AppendLine(stream.ReadLine()) |> ignore
        [ tfProcess.StandardOutput; tfProcess.StandardError ] |> List.iter flushStream

    flushContents 
    tfProcess.WaitForExit()
    flushContents

    tfProcess, sb.ToString()
let processToResult (p:Process, response) =
    match p.ExitCode with
    | 0 -> Ok response
    | _ -> Error response

/// Executes a generic AZ CLI command.
let terraform = executeTerraform >> processToResult

let executeVersion = terraform "version"

let printer s = printfn "%s" s
    
//[<EntryPoint>]
//let main argv =
//    let result = executeVersion
//    match result with
//    | Ok r -> printfn "%s" r
//    | Error r -> printfn "%s" r
//    0 // return an integer exit code

