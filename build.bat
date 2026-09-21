@echo off
setlocal
cd /d "%~dp0"

where dotnet >nul 2>nul
if errorlevel 1 (
    echo [BookieTrader] .NET SDK was not found.
    echo Install the .NET 10 SDK, then run this file again.
    pause
    exit /b 1
)

dotnet restore BookieTrader.csproj
if errorlevel 1 goto :failed

dotnet build BookieTrader.csproj -c Release --no-restore
if errorlevel 1 goto :failed

echo.
echo Build complete.
echo Release ZIP: ReleaseZip\Michael-BookieTrader-0.1.0.zip
pause
exit /b 0

:failed
echo.
echo Build failed.
pause
exit /b 1
