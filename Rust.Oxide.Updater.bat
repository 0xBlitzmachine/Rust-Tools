:: github/0xBlitzmachine
:: Use this Script AFTER you updated your Server using SteamCMD
@echo off
echo Starting Batch Script v.0.1 - Github/0xBlitzmachine

:: Define paths and filenames
set "OXIDE_RUST_GITHUB_PATH=https://umod.org/games/rust/download?tag=public"
set "DOWNLOAD_FILENAME=Oxide.Rust.zip"
set "DOWNLOAD_PATH=C:\Users\blitz\Downloads\"
set "DOWNLOAD_FULLPATH=%DOWNLOAD_PATH%%DOWNLOAD_FILENAME%"
:: In my case, this is where the fresh installed Facepunch Server Files are located.
set "RUST_SERVER_PATH=C:\Users\blitz\Documents\Rust\Development\Server\"

:: Check if the download path exists, create it if it doesn't
if not exist "%DOWNLOAD_PATH%" (
    echo Your set download path does not exist.
    echo Creating Download Path - %DOWNLOAD_PATH%
    mkdir "%DOWNLOAD_PATH%"
)

echo Trying to download Oxide for Rust...

:: Download the ZIP file using curl
curl -L -o "%DOWNLOAD_FULLPATH%" "%OXIDE_RUST_GITHUB_PATH%"

:: Check if the download succeeded
if %ERRORLEVEL% neq 0 (
    echo Failed to download Oxide.
    pause
    exit /b 1
)

echo Download completed successfully!

:: Verify that the ZIP file exists before attempting extraction
if not exist "%DOWNLOAD_FULLPATH%" (
    echo File does not exist? Motherfucker just vanished!
    pause
    exit /b 1
)

:: Extract the ZIP file to the server path, overwriting existing files
echo Extracting %DOWNLOAD_FILENAME% to %RUST_SERVER_PATH%
powershell -command "Expand-Archive -Path '%DOWNLOAD_FULLPATH%' -DestinationPath '%RUST_SERVER_PATH%' -Force"

:: Check if extraction succeeded by seeing if the destination folder now has content
if %ERRORLEVEL% neq 0 (
    echo Extraction failed. Please check the ZIP file or extraction path.
    pause
    exit /b 1
)

:: Clean up by deleting the ZIP file
del "%DOWNLOAD_FULLPATH%"
echo Extraction completed and ZIP file deleted.
pause