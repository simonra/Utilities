param
(
    [Parameter(Mandatory=$True, Position=0, ValueFromPipeline=$True)]
    [string]
    $pathToFile
)

Process
{
    Write-Host "Processing $pathToFile"
    # $fullName = [System.IO.Path]::GetFullPath($pathToFile)
    # Cannot efficiently use the .Net `GetFullPath` method here, because it uses the `[System.Environment]::CurrentDirectory` variable, that does not follow the shells working directory. For details see https://stackoverflow.com/a/33310654/2890086 .
    $fullName = Resolve-Path $pathToFile
    Write-Debug "Full path user supplied: $fullName"
    $folderName = [System.IO.Path]::GetDirectoryName($fullName)
    Write-Debug "Directory is: $folderName"
    $baseName = [System.IO.Path]::GetFileNameWithoutExtension($pathToFile)
    $targetFileName = "$baseName.mp3"
    $targetPath = [System.IO.Path]::Combine($folderName, $targetFileName)
    # vlc -I dummy $pathToFile ":sout=#transcode{acodec=mpga,ab=192}:std{dst=$targetPath,access=file}" vlc://quit

    # Notes on this use of ffmpeg: the `-vn` option specifies that there is to be no video.
    ffmpeg -i $pathToFile -vn -c:a mp3 $targetPath
    Write-Host "Soundtrack placed here: $targetPath"
}
