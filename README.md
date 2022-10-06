# CSYE6225 Cloud Computing
# Cloud Project
![Professor](https://img.shields.io/badge/professor-Tejas%20Parikh-blue)
![Project](https://img.shields.io/badge/project-Cloud-orange)
# Member
1. Ankur Thakur (NUID: 002927572)

# Installation & Usage
<h3> Clone the repository</h3>
git clone git@github.com:AnkurCloudNeu/webapp.git

# Instructions for Running the SpringBoot Project
1. Open Terminal.
2. First we need to run migration to create database on server, for that run these two commands. First, dotnet ef migrations add InitialCreate. After runnung this command successfully, run dotnet ef database update.
3. Once db gets created, Run "dotnet run" command to run the API project.
4. To test whether your api is running, open postman. The endpoint is http://localhost:5215/healthz.
Note: We are using Dotnet core 5.0 for running the application.
  
# Key highlights
 - Dotnet Core
 - C#
 - Entity Framework Core
 - Postgresql
 - AWS