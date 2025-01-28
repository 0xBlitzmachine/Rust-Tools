@echo off

:: Set the path to steamcmd.exe and the path to where the server files should be located!
set "STEAMCMD_PATH=C:\Users\blitz\Documents\Rust\SteamCMD\steamcmd.exe"
set "RUST_SERVER_PATH=C:\Users\blitz\Documents\Rust\Development\Server"
:: Requires the Rust.Oxide.Updater.bat
set "RUST_OXIDE_UPDATER_BATCH_PATH=C:\Users\blitz\Documents\Rust\Development\Github\Batch Scripts\Rust.Oxide.Updater.bat"
:: Rust
set "APP_ID=258550"

%STEAMCMD_PATH% ^
+force_install_dir %RUST_SERVER_PATH% ^
+login anonymous ^
+app_update %APP_ID% ^
+quit

echo .
echo .
echo Your Rust server files are located @ %RUST_SERVER_PATH%
echo Starting Oxide Updater Script ..
pause

if not exist "%RUST_OXIDE_UPDATER_BATCH_PATH%" (
    echo Rust.Oxide.Updater.bat does not exist .. Stopping Script!
    pause
    exit /b 1
)

cls

call "%RUST_OXIDE_UPDATER_BATCH_PATH%"
