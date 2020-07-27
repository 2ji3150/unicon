set rar="C:\Program Files\WinRAR\Rar.exe"
%rar% m -m5 -md1024m -ep1 -r -idq %1 "%~1\" && rd %1 && exit

:error
if exist %1 rd /s /q %1
exit 1