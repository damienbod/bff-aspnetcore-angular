# BFF security architecture using ASP.NET core and nx Angular

[![.NET and npm build](https://github.com/damienbod/bff-aspnetcore-angular/actions/workflows/dotnet.yml/badge.svg)](https://github.com/damienbod/bff-aspnetcore-angular/actions/workflows/dotnet.yml) [![Build and deploy to Azure Web App](https://github.com/damienbod/bff-aspnetcore-angular/actions/workflows/azure-webapps-dotnet-core.yml/badge.svg?branch=deploy)](https://github.com/damienbod/bff-aspnetcore-angular/actions/workflows/azure-webapps-dotnet-core.yml)

## Setup Server 

In production, the Angular nx project is built into the **wwwroot** of the .NET project.

![BFF production](https://github.com/damienbod/bff-aspnetcore-angular/blob/main/images/bff-arch-production_01.png)

Configure the YARP reverse proxy to match the Angular nx URL. This is only required in development. I always use HTTPS in development and the port needs to match the Angular nx developement env.

```json
 "UiDevServerUrl": "https://localhost:4201",
  "ReverseProxy": {
    "Routes": {
      "route1": {
        "ClusterId": "cluster1",
        "Match": {
          "Path": "{**catch-all}"
        }
      }
    },
    "Clusters": {
      "cluster1": {
        "HttpClient": {
          "SslProtocols": [
            "Tls12"
          ]
        },
        "Destinations": {
          "cluster1/destination1": {
            "Address": "https://localhost:4201/"
          }
        }
      }
    }
  }
```

## Setup Angular nx

Add the certificates to the nx project for example in the **/certs** folder

Update the nx project.json file:

```json
"serve": {
    "executor": "@angular-devkit/build-angular:dev-server",
    "options": {
    "browserTarget": "ui:build",
    "sslKey": "certs/dev_localhost.key",
    "sslCert": "certs/dev_localhost.pem",
    "port": 4201
},
```

Update the outputPath for the (nx build) to deploy the production paths to the wwwroot of the .NET project

```
 "build": {
      "executor": "@angular-devkit/build-angular:browser",
      "outputs": ["{options.outputPath}"],
      "options": {
        "outputPath": "../server/wwwroot",
        "index": "./src/index.html",
        "main": "./src/main.ts",
        "polyfills": ["zone.js"],
        "tsConfig": "./tsconfig.app.json",
        "assets": ["./src/favicon.ico", "./src/assets"],
        "styles": ["./src/styles.scss"],
        "scripts": []
      },
```

**Note** When creating a new Angular nx project, it adds git files as well, delete these as this is not required.

## Setup development

![BFF development](https://github.com/damienbod/bff-aspnetcore-angular/blob/main/images/bff-arch-development_01.png)

We always run in HTTPS

```
nx server --ssl
```

## Azure App Registration setup

The application(s) are deployed as one. 

This is an OpenID Connect confidential client with a user secret or a certification for client assertion.

Use the Web client type on setup.

![BFF Azure registration](https://github.com/damienbod/bff-aspnetcore-angular/blob/main/images/azure-app-registration_01.png)

## github actions build

```yaml

name: .NET and npm build

on:
  push:
    branches: [ "main" ]
  pull_request:
    branches: [ "main" ]

jobs:
  build:
    runs-on: ubuntu-latest

    steps:

      - uses: actions/checkout@v3
      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: 7.0.x

      - name: Restore dependencies
        run: dotnet restore

      - name: npm setup
        working-directory: ui
        run: npm install

      - name: ui-nx-build
        working-directory: ui
        run: npm run build

      - name: Build
        run: dotnet build --no-restore
      - name: Test
        run: dotnet test --no-build --verbosity normal
```

## github actions Azure deployment

See **azure-webapps-dotnet-core.yml**

deployment test server: https://bff-angular-aspnetcore.azurewebsites.net

## Links

https://learn.microsoft.com/en-us/aspnet/core/introduction-to-aspnet-core

https://nx.dev/getting-started/intro

https://github.com/AzureAD/microsoft-identity-web