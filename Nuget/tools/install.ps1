param($installPath, $toolsPath, $package, $project)
$projectFullName = $project.FullName
$debugString = "install.ps1 executing for " + $projectFullName
Write-Host $debugString
 $fileInfo = new-object -typename System.IO.FileInfo -ArgumentList $projectFullName
$projectDirectory = $fileInfo.DirectoryName
$tempDirectory = "temp"
$sourceDirectory = "$projectDirectory\$tempDirectory"
Write-Host $sourceDirectory
 
$destinationDirectory = "$projectDirectory"
Write-Host $destinationDirectory
 
if(test-path $sourceDirectory -pathtype container)
{
 if (test-Path $destinationDirectory\Build.exe){
     Write-Host "Removing old build.exe"
     remove-item $destinationDirectory\Build.exe -recurse
 }

 Write-Host "Copying files from $sourceDirectory to $destinationDirectory"
 robocopy $sourceDirectory $destinationDirectory /XO
 
 Write-Host "Removing $tempDirectory from project."
 $tempDirectoryProjectItem = $project.ProjectItems.Item($tempDirectory)
 $tempDirectoryProjectItem.Remove()

 Write-Host "Deleting $sourceDirectory"
 remove-item $sourceDirectory -recurse
}
 
$debugString = "install.ps1 complete" + $projectFullName
Write-Host $debugString