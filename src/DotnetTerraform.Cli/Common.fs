module DotnetTerraform.Cli.Common

open System
open System.IO
open System.Runtime.InteropServices

type OsInfo = { platform: string; arch: string }

let fileExists path: Result<string, Exception> =
    if File.Exists(path)
    then Ok(path)
    else Error(FileNotFoundException("{0} was not found!", path) :> _)

let (|OperatingSystem|_|) platform () =
    if RuntimeInformation.IsOSPlatform platform
    then Some()
    else None

let getPlatform (): Result<string, Exception> =
    match () with
    | OperatingSystem OSPlatform.Windows -> Ok("windows")
    | OperatingSystem OSPlatform.Linux -> Ok("linux")
    | OperatingSystem OSPlatform.OSX -> Ok("darwin")
    | OperatingSystem OSPlatform.FreeBSD -> Ok("freebsd")
    | _ ->
        Error
            (ArgumentOutOfRangeException
                ("Current operating system isn't supported: {0}", RuntimeInformation.OSDescription) :> _)

let getArchitecture platform: Result<OsInfo, Exception> =
    match platform with
    | Ok p ->
        match RuntimeInformation.OSArchitecture with
        | Architecture.X64 -> Ok({ platform = p; arch = "amd64" })
        | Architecture.X86 -> Ok({ platform = p; arch = "386" })
        | Architecture.Arm -> Ok({ platform = p; arch = "arm" })
        | Architecture.Arm64 ->
            Error
                (ArgumentOutOfRangeException
                    ("ARM 64 isn't a supported architecture. Read more here: https://github.com/hashicorp/terraform/issues/14474") :> _)
        | _ -> Error(ArgumentOutOfRangeException("Current operating system architecture isn't supported.") :> _)
    | Error e -> Error(e) 