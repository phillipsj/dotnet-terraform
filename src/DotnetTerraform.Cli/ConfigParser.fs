module DotnetTerraform.Cli.ConfigParser

open System
open System.IO
open Tomlyn
open Tomlyn.Syntax
open DotnetTerraform.Cli.Common

let getContent path: Result<string, Exception> =
    try
        let contents = File.ReadAllText(path)
        Ok(contents)
    with ex -> Error(ex)

let parseToml (contents: string): Result<DocumentSyntax, Exception> =
    try
        let parsed = Toml.Parse(contents)
        if not parsed.HasErrors
        then Ok(parsed)
        else Error(Exception("The parsed TOML file has errors, cannot continue."))
    with ex -> Error(ex)

let getVersion (toml: DocumentSyntax): Result<string, Exception> =
    try
        let model = toml.ToModel()
        let found, version = model.TryGetValue("version")
        if found
        then Ok(version.ToString())
        else Error(Exception("Key version could not be found in the toml file."))
    with ex -> Error(ex)

let validateConfiguration config: Result<string, Exception> =
    config
    |> Result.bind fileExists
    |> Result.bind getContent
    |> Result.bind parseToml
    |> Result.bind getVersion
