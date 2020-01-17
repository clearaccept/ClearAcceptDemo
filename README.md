# Clear Accept Hosted Fields Demo

## The goal of this demo
This demo project was created as an integration example for the Clear Accept Hosted Fields library.

## Steps to run the application

To run the application locally, follow these steps:
1. Open a command line interface and clone to your local computer using `git`.
```shell
git clone https://github.com/clearaccept/ClearAcceptDemo
```
2. Make sure you have .NET Core 3.0 SDK installed. See instruction from [here](https://dotnet.microsoft.com/download/dotnet-core/3.0).
3. Navigate inside the folder where the repository was cloned, to the demo project folder.
```shell
cd ClearAcceptDemo\ClearAcceptDemo
``` 
4. Copy appsettings.example.json to appsettings.json
```shell
copy appsettings.example.json appsettings.json
``` 
5. Edit appsettings.json and enter your credentials and platform ids in their placeholders:
6. ![Demo Project](docs/images/appsettings.png)
6. Use `dotnet run` in command line to start the demo server.
```shell
dotnet run
```
7. Open any Browser and navigate to https://localhost:44331/ to acces the demo page.


## About the Demo Project
![Demo Project](docs/images/demo-project.png)

This demo platform contains some default fields that contain non sensitive data which are on the demo platform host,
and the 3 fields (Card, Expiration date, Cvv) in 3 different iframes. These three fields will contain 
sensitive data, so the content of the iframes are hosted on https://sandboxm-hosted.clearcourse.systems/ and the platform
itself cannot access the data inside of them.
You can test it by filling the input fields with valid data and clicking on the 'Pay now' button, and you will
be redirected to the result page.

![Result Page](docs/images/result-page.png)

On the results page, the STATUS indicator shows the status of the payment request
created with the data you entered previously. This page also contains two text areas to allow you
to view the request and response from the payments gateway.

To start a new payment click on 'start new payment' and you will  be redirected to the payment form 
on the main page. 

