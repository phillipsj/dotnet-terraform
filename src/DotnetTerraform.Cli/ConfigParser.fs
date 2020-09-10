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
        Ok(parsed)
    with ex -> Error(ex)
    

let validateConfiguration config: Result<DocumentSyntax, Exception> =
    config
    |> Result.bind fileExists
    |> Result.bind getContent
    |> Result.bind parseToml
