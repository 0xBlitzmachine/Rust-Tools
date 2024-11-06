:: Should be located whereever your Rust server files are located.
@echo off
cls
:start
echo Starting server...

RustDedicated.exe -batchmode -nographics ^
+rcon.ip 0.0.0.0 ^
+rcon.port 28016 ^
+rcon.password "bl111tz" ^
+server.ip 0.0.0.0 ^
+server.port 28015 ^
+server.maxplayers 10 ^
+server.hostname "Local DEV" ^
+server.identity "dev-server" ^
+server.level "Procedural Map" ^
+server.seed 12345 ^
+server.worldsize 4000 ^
+server.saveinterval 300 ^
+server.globalchat true ^
+server.description "Powered by Oxide" ^
+server.headerimage "http://i.imgur.com/xNyLhMt.jpg" ^
+server.url "https://oxidemod.org" ^
-logfile server-log.txt

echo.
echo Restarting server...
timeout /t 10
echo.
goto start