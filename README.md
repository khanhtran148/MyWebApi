# Introduction
MyWebApi

# Getting Started
Install packages
- https://dotnet.microsoft.com/en-us/download/dotnet/6.0
- https://github.com/RicoSuter/NSwag/wiki/NSwagStudio
- Fist time running project:  run command `dotnet tool restore` when open powershell at root folder of project

# SAGA Pattern
- Choreography-based saga
- Orchestration-based saga

# Flow
User send request -> Api receive -> Submit -> Process -> Sent mail -> Done (Final state)

# Masstransit saga state
0 - None, 1 - Initial, 2 - Final, 3 - Processed, 4 - MailSent
