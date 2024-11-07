@echo off

:: Set the path to steamcmd.exe and the path to where the server files should be located!
set "STEAMCMD_PATH=C:\Users\blitz\Documents\Rust\SteamCMD\steamcmd.exe"
set "RUST_SERVER_PATH=C:\Users\blitz\Documents\Rust\Development\Server"
:: Rust
set "APP_ID=258550"

%STEAMCMD_PATH% ^
+force_install_dir %RUST_SERVER_PATH% ^
+login anonymous ^
+app_update 258550 ^
+quit
