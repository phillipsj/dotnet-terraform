<#
.Synopsis
	Build script, https://github.com/nightroman/Invoke-Build
#>

param(
	[ValidateSet('Debug', 'Release')]
	[string]$Configuration = 'Release',
	[string]$Version = "1.5.2",
	[string]$NuGetApiKey = (property NuGetApiKey "")
)

$artifactsPath = "./artifacts"
$platforms = @("linux_amd64", "linux_arm64", "windows_amd64", "darwin_arm64")

# Synopsis: Remove files.
task clean {
	remove artifacts
}

# Synopsis: Setup
task setup clean, {
	if(!(Test-Path -PathType container -Path $artifactsPath)) {
      New-Item -ItemType Directory -Path $artifactsPath
    }
}

# Synopsis: Download Terraform Version
task download setup, {
	Set-Location $artifactsPath
	$checksumsUrl = "https://releases.hashicorp.com/terraform/$($Version)/terraform_$($Version)_SHA256SUMS" 
	Invoke-RestMethod -Uri $checksumsUrl -OutFile "terraform_$($Version)_SHA256SUMS"

	foreach($platform in $platforms) {
		$releaseUrl = "https://releases.hashicorp.com/terraform/$($Version)/terraform_$($Version)_$($platform).zip"
		Invoke-RestMethod -Uri $releaseUrl -OutFile "terraform_$($Version)_$($platform).zip"
	}
}

# Synopsis: Validate Checksums
task checksum download,{
	Set-Location $artifactsPath
    $checksums = Get-Content -Path "./terraform_$($Version)_SHA256SUMS"
	foreach($platform in $platforms) {
		$file = "terraform_$($Version)_$($platform).zip"
		$fileHash = (Get-FileHash "./$($file)" -Algorithm SHA256).Hash.ToLower()
		$valid = $checksums -contains "$($fileHash)  $($file)"
		assert($valid) "Checksum for $($file) didn't match the checksum file value tested: $($fileHash)"
	}
}

# Synopsis: Stage Files
task stage checksum,{
	Set-Location $artifactsPath
	foreach($platform in $platforms) {
		$file = "terraform_$($Version)_$($platform).zip"
		Expand-Archive -Path "./$($file)" -DestinationPath "./$($platform)"
	}
}

# Synopsis: Build project.
task build stage, {
    dotnet build -c $Configuration /p:Version=$Version
}

# Synopsis: Create NuGet package.
task pack build, {
    dotnet pack /p:PackageVersion=$Version --output $artifactsPath --no-build -c $Configuration
}

# Synopsis: Publish NuGet package.
task publish pack, {
    $packageName = "dotnet-terraform.$($Version).nupkg"
    dotnet nuget push $packageName --api-key $NuGetApiKey --source https://api.nuget.org/v3/index.json
}

# Synopsis: Default task.
task . build
