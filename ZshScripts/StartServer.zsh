#!/usr/bin/zsh

while true; do

echo "Starting Server .."

exec ./RustDedicated -batchmode -nographics \
-server.ip 0.0.0.0 \
-server.port 28015 \
-server.hostname "Linux Env - Dev Server" \
-server.identity "dev-server" \
-server.maxplayers 5 \
-server.worldsize 1000 \
-server.seed 12345 \
-server.saveinterval 200 \
-server.level "Procedural Map" \
-server.secure 0 \
-server.encryption 0 \

echo "Restarting ..."
sleep 5
done
