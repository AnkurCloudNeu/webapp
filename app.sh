#!/bin/bash

sleep 30

sudo apt-get update
sudo chown -R ubuntu:ubuntu /home/ubuntu/.ssh
sudo chmod 700 /home/ubuntu/.ssh
sudo apt-get install apt-transport-https
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb
sudo apt-get install -y dotnet-runtime-6.0
sudo apt-get install -y jq
sudo yum install amazon-cloudwatch-agent
export PATH="$PATH:$HOME/.dotnet/tools/"
dotnet tool install --global dotnet-ef
sudo apt install unzip -y
cd ~/ && unzip webapp.zip
cd ~/webapp
dotnet publish ./WebApp.CloudApi/WebApp.CloudApi.csproj --configuration Release
sudo mv /tmp/webapp.service /etc/systemd/system/webapp.service