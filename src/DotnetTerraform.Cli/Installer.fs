module DotnetTerraform.Cli.Installer

open System
open System.IO
open System.IO.Compression
open System.Net
open DotnetTerraform.Cli.Common
open System.Runtime.InteropServices

type TfInfo =
    { platform: string
      arch: string
      version: string }

let buildUrl tfInfo: Result<string, Exception> =
    try
        let url =
            sprintf
                "https://releases.hashicorp.com/terraform/%s/terraform_%s_%s_%s.zip"
                tfInfo.version
                tfInfo.version
                tfInfo.platform
                tfInfo.arch

        Ok(url)
    with ex -> Error(ex)

let downloadFile (url: string): Result<string, Exception> =
    try
        let destination = "terraform.zip" // Need to make the path that I want here.
        use wc = new WebClient()
        wc.DownloadFile(url, destination)
        Ok(destination)
    with ex -> Error(ex)

let tfDirectory =
    Path.Combine(Environment.CurrentDirectory, ".tf")

let determineExecutable: string =
    if (RuntimeInformation.IsOSPlatform OSPlatform.Windows)
    then "terraform.exe"
    else "terraform"

let executable: string =
    Path.Combine(tfDirectory, determineExecutable)

let extractFile (zipPath: string): Result<string, Exception> =
    try
        if not (Directory.Exists(tfDirectory))
        then Directory.CreateDirectory(tfDirectory) |> ignore
        ZipFile.ExtractToDirectory(zipPath, tfDirectory)
        Ok(executable)
    with ex -> Error(ex)

let getOsInfo: Result<OsInfo, Exception> = () |> getPlatform |> getArchitecture

let installTerraform tfInfo: Result<string, Exception> =
    tfInfo
    |> Result.bind buildUrl
    |> Result.bind downloadFile
    |> Result.bind extractFile