#r "paket:
nuget Fake.DotNet.Cli
nuget Fake.IO.FileSystem
nuget Fake.Core.Target //"

#load ".fake/build.fsx/intellisense.fsx"
open Fake.Core
open Fake.DotNet
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators
open Fake.Core.TargetOperators

Target.initEnvironment ()

Target.create "Clean" (fun _ -> !! "src/**/bin" ++ "src/**/obj" |> Shell.cleanDirs)

Target.create "Build" (fun _ -> !! "src/**/*.*proj" |> Seq.iter (DotNet.build id))

Target.create "Push" (fun _ ->
    let apiKey =
        Environment.environVarOrFail "NUGET_API_KEY"

    let setNugetPushParams (defaults: NuGet.NuGet.NuGetPushParams) =
        { defaults with
              DisableBuffering = true
              ApiKey = Some apiKey }

    let setParams (defaults: DotNet.NuGetPushOptions) =
        { defaults with
              PushParams = setNugetPushParams defaults.PushParams }

    !! "src/**/nupkg/*.nupkg"
    |> Seq.iter (fun nupkg -> DotNet.nugetPush setParams nupkg))

Target.create "All" ignore

"Clean" ==> "Build" ==> "All"

Target.runOrDefault "All"
