set winrar="C:\Program Files\WinRAR\WinRAR.exe"
%winrar% x -ai -ibck %1 "%~dpn1$TMP\" && exit

:error
if exist "%~dpn1$TMP" rd /s /q "%~dpn1$TMP"
exit 1