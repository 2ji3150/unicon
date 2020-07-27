set cwebp=D:\FIONE\bin\libwebp\cwebp
set inFile=%1
set outFile="%~n1.webp"
set tmpFile="%~dpn1.webp.tmp"

%cwebp% -mt -lossless -z 9 -m 6 -q 100 %inFile% -o %tmpFile% && del /f /q %inFile% && ren %tmpFile% %outFile% && exit

:error
if exist %tmpFile% del /f /q %tmpFile%
exit 1