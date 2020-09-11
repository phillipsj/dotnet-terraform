open System
open DotnetTerraform.Cli.Common
open DotnetTerraform.Cli.Commands
open DotnetTerraform.Cli.ConfigParser
open DotnetTerraform.Cli.Installer
open Argu

type CliError = | ArgumentsNotSpecified

let getExitCode result =
    match result with
    | Ok () -> 0
    | Error err ->
        match err with
        | ArgumentsNotSpecified -> 1

[<EntryPoint>]
let main argv =
    let errorHandler =
        ProcessExiter
            (colorizer =
                function
                | ErrorCode.HelpText -> None
                | _ -> Some ConsoleColor.Red)

    let parser =
        ArgumentParser.Create<CmdArgs>(programName = "dotnet-terraform", errorHandler = errorHandler)

    match parser.ParseCommandLine argv with
    | p when p.Contains(Install) ->
        let configFile = "terraform.toml"
        let version = validateConfiguration (Ok(configFile))
        match version with
        | Ok v ->
            let osInfo = getOsInfo
            match osInfo with
            | Ok o ->
                let tfInfo =
                    { platform = o.platform
                      arch = o.arch
                      version = v }

                let exePath = installTerraform (Ok(tfInfo))
                match exePath with
                | Ok p -> p |> printfn "Terraform was successfully installed at: %s"
                | Error e -> e.Message |> printfn "Error: %s"
            | Error e -> e.Message |> printfn "Error: %s"
        | Error e -> e.Message |> printfn "Error: %s"
        Ok()
    | p when p.Contains(Init) ->
        p.GetResults(Init).ToString()
        |> printfn "Init command with the following %s"
        Ok()
    | _ ->
        parser.PrintUsage() |> printfn "%s"
        Error ArgumentsNotSpecified
    |> getExitCode
