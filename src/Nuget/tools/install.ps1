param($installPath, $toolsPath, $package, $project)

$projectFullName = $project.FullName
Write-Host "Install.ps1 executing for $projectFullName."

$fileInfo = new-object -typename System.IO.FileInfo -ArgumentList $projectFullName
$projectDirectory = $fileInfo.DirectoryName
$tempDirectory = "temp"
$sourceDirectory = "$projectDirectory\$tempDirectory"
$destinationDirectory = "$projectDirectory"
$buildscript = "BuildScript.cs"

Write-Host "Source dir: $sourceDirectory"
Write-Host "Destination dir: $destinationDirectory"
 
if(test-path $sourceDirectory -pathtype container)
{
    if (test-Path $destinationDirectory\Build.exe)
    {
        Write-Host "Removing old build.exe"
        remove-item $destinationDirectory\Build.exe -recurse
        
        if (test-path $sourceDirectory\Flubu.exe)
        {
            Write-Host "WARNING: build.exe was renamed to Flubu.exe"
        }
    }

    if (test-Path $destinationDirectory\Flubu.exe)
    {
        Write-Host "Removing old Flubu.exe"
        remove-item $destinationDirectory\Flubu.exe -recurse
    }

    Write-Host "Copying files from $sourceDirectory to $destinationDirectory"
    # /XF skips BuildScript.cs
    robocopy $sourceDirectory $destinationDirectory /XO /XF "$sourceDirectory\$buildscript"

    $buildscriptInProject = $project.ProjectItems | ? { $_.Properties.Item("Filename").Value -eq "$buildscript" }
    if ($buildscriptInProject -eq $null)
    {
        Write-Host "Adding default $buildscript."
        (Get-Interface $project.ProjectItems "EnvDTE.ProjectItems").AddFromFileCopy("$sourceDirectory\$buildscript")
    }
    else
    {
        Write-Host "File $buildscript already exists, preserving existing file."
    }

    Write-Host "Deleting $tempDirectory."
    $project.ProjectItems.Item($tempDirectory).Delete()
}
 
Write-Host "install.ps1 complete for $projectFullName."
