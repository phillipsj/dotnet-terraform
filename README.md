[![NuGet](https://buildstats.info/nuget/dotnet-terraform)](https://www.nuget.org/packages/dotnet-terraform)

## Terraform as a .NET CLI Tool

Terraform packaged as a .NET CLI tool available for Windows x64, Linux x64, Linux arm64, and MacOS arm64. Only LTS 
versions will be supported. If any requests to support additional versions are submitted that could change.

## Why

I created this project to solve two problems that I was experiencing and I hope this solves the same issues for you. The
first issue is related to needing a simple and effective way to install Terraform on a target environment. I don't like 
my logic tied up in platform specific YAML steps since I can't often run that locally. By wrapping Terraform as a .NET 
CLI tool, I can ensure that I can easily install Terraform locally. The second benefit is ensuring that I am always 
using the same version of Terraform that I used to write the infrastructure locally and on the build server. Installing
Terraform as a [local tool](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-tool-install#local-tools) ensures I always have the correct version that a specific project requires.

## Getting Started

You can install this project as either a global tool or a local tool. The intention of this project is as use as a local
tool while a global install works just as well.

### Local Install

First step is to ensure you have a manifest created. This will also need to be checked into source control.

```Bash
dotnet new tool-manifest
```

Now install the tool using the version you want.

```Bash
dotnet install dotnet-terraform --version "*-rc*"
```

### Global Install

The command is:

```Bash
dotnet install -g dotnet-terraform --version "*-rc*"
```

## Usage

This tool is just a wrapper around Terraform. You can pass the exact commands and arguments and it all should work. The
tool command is `dotnet-terraform`. This ensures that there are no conflicts with Terraform installed by other methods.
This tool relies on how the naming conventions work as specified here for [global tools](https://learn.microsoft.com/en-us/dotnet/core/tools/global-tools#invoke-a-global-tool) 
and [local tools](https://learn.microsoft.com/en-us/dotnet/core/tools/global-tools#invoke-a-local-tool). My advice is to
always use the following command style `dotnet terraform` and you should get the desired behavior you expect.

Examples:

```Bash
$ dotnet terraform version
Terraform v1.5.0
on linux_amd64

Your version of Terraform is out of date! The latest version
is 1.5.2. You can update by downloading from https://www.terraform.io/downloads.html
```

```Bash
$ dotnet terraform fmt --help
Usage: terraform [global options] fmt [options] [target...]

  Rewrites all Terraform configuration files to a canonical format. Both
  configuration files (.tf) and variables files (.tfvars) are updated.
  JSON files (.tf.json or .tfvars.json) are not modified.

  By default, fmt scans the current directory for configuration files. If you
  provide a directory for the target argument, then fmt will scan that
  directory instead. If you provide a file, then fmt will process just that
  file. If you provide a single dash ("-"), then fmt will read from standard
  input (STDIN).

  The content must be in the Terraform language native syntax; JSON is not
  supported.
...
```

## Credits

Terraform for being an awesome tool.