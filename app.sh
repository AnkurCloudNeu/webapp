#!/bin/bash

sleep 30

sudo apt-get update
sudo chown -R ubuntu:ubuntu /home/ubuntu/.ssh
sudo chmod 700 /home/ubuntu/.ssh
sudo apt-get install apt-transport-https
sudo apt-get install -y dotnet6
export PATH="$PATH:$HOME/.dotnet/tools/"
dotnet tool install --global dotnet-ef
sudo apt install unzip -y
cd ~/ && unzip webapp.zip

cd ~/webapp
dotnet publish ./WebApp.CloudApi/WebApp.CloudApi.csproj --configuration Release

sudo mv /tmp/webapp.service /etc/systemd/system/webapp.service
sudo systemctl enable webapp.service
sudo systemctl start webapp.service
sudo systemctl status webapp.service