module DotnetTerraform.Cli.TfCommands

open System.Diagnostics
open System.IO
open System.Text
open DotnetTerraform.Cli.Installer

let executeTerraform arguments =
    let tfProcess =
        ProcessStartInfo
            (FileName = executable,
             Arguments = "version",
             UseShellExecute = false,
             RedirectStandardOutput = true,
             RedirectStandardError = true)
        |> Process.Start

    let sb = StringBuilder()

    let flushContents =
        let flushStream (stream: StreamReader) =
            while not stream.EndOfStream do
                sb.AppendLine(stream.ReadLine()) |> ignore

        [ tfProcess.StandardOutput
          tfProcess.StandardError ]
        |> List.iter flushStream

    flushContents
    tfProcess.WaitForExit()
    flushContents

    tfProcess, sb.ToString()

let processToResult (p: Process, response) =
    match p.ExitCode with
    | 0 -> Ok response
    | _ -> Error response

let terraform = executeTerraform >> processToResult

let executeVersion = terraform "version"

let printer s = printfn "%s" s
