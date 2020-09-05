module DotnetTerraform.Cli.ConfigParser

open System
open System.IO
open Tomlyn
open Tomlyn.Syntax

let tomlExists path: Result<string, Exception> =
    if File.Exists(path)
    then Ok(path)
    else Error(FileNotFoundException("Terraform.toml was not found!") :> _)

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
    |> Result.bind tomlExists
    |> Result.bind getContent
    |> Result.bind parseToml
