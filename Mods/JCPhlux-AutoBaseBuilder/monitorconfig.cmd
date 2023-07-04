@echo off
set "sourcePath=C:\Dev\AutoBaseBuilder\AutoBaseBuilder\Config"
set "destinationPath=C:\7D2D\Custom\MyMods\Mods\JCPhluxAutoBaseBuilder\Config"
set "waitTimeSeconds=10"
set "exitKey=X"

:monitor

robocopy "%sourcePath%" "%destinationPath%" /E /PURGE /XO

choice /c %exitKey%q /n /t %waitTimeSeconds% /d "q" /m "Press %exitKey% to exit, or any other key to continue:"

if errorlevel 1 (
    goto :monitor
)