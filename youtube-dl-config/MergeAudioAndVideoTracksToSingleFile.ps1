param
(
    [Parameter(Mandatory=$True, Position=0, ValueFromPipeline=$True)]
    [string]
    $pathToVideoFile,
    [Parameter(Mandatory=$True, Position=1, ValueFromPipeline=$True)]
    [string]
    $pathToSoundTracFile
)

Process
{
    # ToDo: Rewrite to fit in both files instead of just audio-track extraction and change to ffmpeg from VLC.
    Write-Host "Merging video file $pathToVideoFile and soundtrack file $pathToSoundTracFile into single file."
    # $fullName = [System.IO.Path]::GetFullPath($pathToVideoFile)
    # Cannot efficiently use the .Net `GetFullPath` method here, because it uses the `[System.Environment]::CurrentDirectory` variable, that does not follow the shells working directory. For details see https://stackoverflow.com/a/33310654/2890086 .
    $fullNameVideoFile = Resolve-Path $pathToVideoFile
    Write-Debug "Full path user supplied for video: $fullNameVideoFile"
    $folderNameVideoFile = [System.IO.Path]::GetDirectoryName($fullNameVideoFile)
    Write-Debug "Directory of video is: $folderNameVideoFile"
    $baseNameVideo = [System.IO.Path]::GetFileNameWithoutExtension($pathToVideoFile)
    $targetFileName = "$baseNameVideo-merged.webm"
    $targetPath = [System.IO.Path]::Combine($folderNameVideoFile, $targetFileName)
    # Note: Setting the audio codec (-c:a) to `copy` and the video codec (-c:v) to `copy` gives the best results due to no recompression.
    # However, this requires the formats to be compatible with the target container (in this case webm).
    # As of 2021-01-01 you can use `libopus` for the audio codec to convert to opus, and `libvpx-vp9` to convert the video codec to VP9.
    # Example: `ffmpeg -i $pathToVideoFile -i $pathToSoundTracFile -c:a libopus -c:v libvpx-vp9 $targetPath`.
    ffmpeg -i $pathToVideoFile -i $pathToSoundTracFile -c:a copy -c:v copy $targetPath
    Write-Host "Merged video and audio file placed here: $targetPath"
}
