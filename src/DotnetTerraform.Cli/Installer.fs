module DotnetTerraform.Cli.Installer

open System
open System.IO.Compression
open System.Net

type OsInfo =
    { name: string
      arch: string
      version: string }

let buildUrl osInfo: Result<string, Exception> =
    try
        let url =
            sprintf
                "https://releases.hashicorp.com/terraform/%s/terraform_%s_%s_%s.zip"
                osInfo.version
                osInfo.version
                osInfo.name
                osInfo.arch

        Ok(url)
    with ex -> Error(ex)

let downloadFile (url: string): Result<string, Exception> =
    try
        let destination = "terraform.zip" // Need to make the path that I want here.
        use wc = new WebClient()
        wc.DownloadFile(url, destination)
        Ok(destination)
    with ex -> Error(ex)

let extractFile zipPath: Result<string, Exception> =
    try
        ZipFile.ExtractToDirectory(zipPath, Environment.CurrentDirectory)
        let exePath = "terraform.exe" // Need to extract determine the full exe path
        Ok(exePath)
    with ex -> Error(ex)

let installTerraform osInfo: Result<string, Exception> =
    osInfo
    |> Result.bind buildUrl
    |> Result.bind downloadFile
    |> Result.bind extractFile