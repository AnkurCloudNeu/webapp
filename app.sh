#!/bin/bash

sleep 30

sudo apt update

sudo apt-get install -y dotnet6
export PATH="$PATH:$HOME/.dotnet/tools/"
dotnet tool install --global dotnet-ef
sudo apt -y install postgresql postgresql-contrib

sudo apt install zip unzip -y
cd ~/ && unzip webapp.zip
cd ~/webapp && npm i --only=prod

sudo mv /tmp/webapp.service /etc/systemd/system/webapp.service
sudo systemctl enable webapp.service
sudo systemctl start webapp.service
sudo service postgresql 
sudo su postgres <<EOF
psql -c "ALTER USER postgres PASSWORD 'postgres'"
EOF
dotnet build
dotnet ef migrations add InitialCreate --project ./WebApp.CloudApi/WebApp.CloudApi.csproj
dotnet ef database update --project ./WebApp.CloudApi/WebApp.CloudApi.csproj
dotnet run --urls http://0.0.0.0:8080 --project ./WebApp.CloudApi/WebApp.CloudApi.csproj