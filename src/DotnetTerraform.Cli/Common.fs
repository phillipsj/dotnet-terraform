module DotnetTerraform.Cli.Common

open System
open System.IO
open System.Runtime.InteropServices

let fileExists path: Result<string, Exception> =
    if File.Exists(path)
    then Ok(path)
    else Error(FileNotFoundException("{0} was not found!", path) :> _)
    
let (|OperatingSystem|_|) platform () =
            if RuntimeInformation.IsOSPlatform platform then Some() else None
            
let getPlatform : Result<string, Exception> =
        match() with
        | OperatingSystem OSPlatform.Windows -> Ok("windows")
        | OperatingSystem OSPlatform.Linux -> Ok("linux")
        | OperatingSystem OSPlatform.OSX -> Ok("darwin")
        | OperatingSystem OSPlatform.FreeBSD -> Ok("freebsd")
        | _ -> Error(ArgumentOutOfRangeException("Current operating system isn't supported: {0}", RuntimeInformation.OSDescription) :> _)

let getArchitecture : Result<string, Exception> =
    match RuntimeInformation.OSArchitecture with
    | Architecture.X64 -> Ok("amd64")
    | Architecture.X86 -> Ok("386")
    | Architecture.Arm -> Ok("arm")
    | Architecture.Arm64 -> Error(ArgumentOutOfRangeException("ARM 64 isn't a supported architecture. Read more here: https://github.com/hashicorp/terraform/issues/14474") :> _)
    | _ -> Error(ArgumentOutOfRangeException("Current operating system architecture isn't supported.") :> _)