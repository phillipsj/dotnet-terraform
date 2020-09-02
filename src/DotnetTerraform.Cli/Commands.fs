module DotnetTerraform.Cli.Commands

open Argu

type CmdArgs =
    | [<CliPrefix(CliPrefix.None)>] Install
    | [<CliPrefix(CliPrefix.None)>] Init of tfargs:string list
    | [<CliPrefix(CliPrefix.None)>] Apply of tfargs:string list
    | [<CliPrefix(CliPrefix.None)>] Plan of tfargs:string list
    | [<CliPrefix(CliPrefix.None)>] Fmt of tfargs:string list
with
    interface IArgParserTemplate with
        member this.Usage =
            match this with
            | Install _ -> "Installs the version of Terraform specified in terraform.toml"
            | Init _ -> "Executes terraform init"
            | Apply _ -> "Executes terraform apply"
            | Plan _ -> "Executes terraform plan"
            | Fmt _ -> "Executes terraform fmt"