set mozjpeg=D:\FIONE\bin\mozjpeg\jpegtran-static.exe
set inFile=%1
set outFile="%~n1.jpg"
set tmpFile="%~dpn1.jpg.tmp"

%mozjpeg% -copy none -outfile %tmpFile% %inFile% && del /f /q %inFile% && ren %tmpFile% %outFile% && exit

:error
if exist %tmpFile% del /f /q %tmpFile%
exit 1