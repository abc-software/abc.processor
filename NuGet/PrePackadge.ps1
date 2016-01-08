# Uncomment below and modify for testing
# $tfsDropFolder = "\\dev\Deployment\Test\Diagnostic (Main)\Diagnostic (Main)_20120222.17"
# $tfsSourcesFolder = "C:\Builds\1\Abc.Diagnostic\Diagnostic (Main)\Sources"
# $tfsBinariesFolder = "C:\Builds\1\Abc.Diagnostic\Diagnostic (Main)\Binaries"
# $tfsNuGetPrePackageFolder = "NuGetPrePackage"

# Calculate where the files will be copied for the NuGet Packaging process
if ([IO.Path]::IsPathRooted($tfsNuGetPrePackageFolder))
{
	$nugetPrePackageFolder = $tfsNuGetPrePackageFolder
}
else
{
	$nugetPrePackageFolder = Join-Path $tfsDropFolder $tfsNuGetPrePackageFolder
}

# Create some variables that will be used to create the package structure
$libFolder = "lib"
$contentFolder = "content" 
$toolsFolder = "tools" 

$net40 = "net40"
$net35 = "net35"
$net20 = "net20"

# Function to create a subfolder with some error checking and validation
Function Create-FrameworkFolder
{
	Param([string]$rootPath = $(throw "$rootPath required."), [string]$subFolder)

   	if ([System.String]::IsNullOrEmpty($subFolder))
	{
		$folderToCreate = $rootPath
	}
	else 
	{
		$folderToCreate = Join-Path $rootPath $subFolder
	}
	
    if (![IO.Directory]::Exists($folderToCreate)) 
	{
    		New-Item $folderToCreate -ItemType directory
	}
}

# Structure to Create:
# NuGetPrePackage
#                 \ content
#                           \ models
#                 \ lib
#                           \ net20
#                           \ net35
#                           \ net40
#                 \ tools

Create-FrameworkFolder -rootPath $nugetPrePackageFolder
#Create-FrameworkFolder -rootPath $nugetPrePackageFolder -subFolder $contentFolder
Create-FrameworkFolder -rootPath $nugetPrePackageFolder -subFolder $libFolder
#Create-FrameworkFolder -rootPath $nugetPrePackageFolder -subFolder $toolsFolder

$prePackageLibFolder = Join-Path $nugetPrePackageFolder $libFolder
Create-FrameworkFolder -rootPath $prePackageLibFolder -subFolder $net20
#Create-FrameworkFolder -rootPath $prePackageLibFolder -subFolder $net35
#Create-FrameworkFolder -rootPath $prePackageLibFolder -subFolder $net40


# Identify the source location(s) for the files that were built as part of the 
# TFS Build Process
$net20Folder = Join-Path $tfsDropFolder "Processor"

# Copy all the files into position so NuGet can do the packaging
$dest = Join-Path $prePackageLibFolder $net20
Copy-Item "$net20Folder\*Diagnostic.dll" -Destination $dest
Copy-Item "$net20Folder\*Diagnostic.xml" -Destination $dest