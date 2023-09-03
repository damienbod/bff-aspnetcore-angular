# BFF security architecture using ASP.NET core and nx Angular

[![.NET and npm build](https://github.com/damienbod/bff-aspnetcore-angular/actions/workflows/dotnet.yml/badge.svg)](https://github.com/damienbod/bff-aspnetcore-angular/actions/workflows/dotnet.yml) [![Build and deploy to Azure Web App](https://github.com/damienbod/bff-aspnetcore-angular/actions/workflows/azure-webapps-dotnet-core.yml/badge.svg?branch=deploy)](https://github.com/damienbod/bff-aspnetcore-angular/actions/workflows/azure-webapps-dotnet-core.yml)

## Setup Server 

The ASP.NET Core project is setup to run in development and production. In production, it uses the Angular production build deployed to the wwwroot. In development, it uses MS YARP reverse proxy to forward requests.

> [!IMPORTANT]  
> In production, the Angular nx project is built into the **wwwroot** of the .NET project.

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

> [!NOTE]  
> default Angular using port 4200, this needs to match the YARP reverse proxy settings for development.

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

> [!NOTE]  
> When creating a new Angular nx project, it adds git files as well, delete these as this is not required.

## Setup development

![BFF development](https://github.com/damienbod/bff-aspnetcore-angular/blob/main/images/bff-arch-development_01.png)

> [!NOTE]  
> Always run in HTTPS, both in development and production

```
nx server --ssl
```

## Azure App Registration setup

The application(s) are deployed as one. 

This is an OpenID Connect confidential client with a user secret or a certification for client assertion.

Use the Web client type on setup.

![BFF Azure registration](https://github.com/damienbod/bff-aspnetcore-angular/blob/main/images/azure-app-registration_01.png)

The OpenID Connect client is setup using **Microsoft.Identity.Web**. This implements the Microsoft Entra ID client. I have created downstream APIs using the OBO flow and a Microsoft Graph client. This could be replace with any OpenID Connect client and requires no changes in the frontend part of the solution.

```csharp
var scopes = configuration.GetValue<string>("DownstreamApi:Scopes");
string[] initialScopes = scopes!.Split(' ');

services.AddMicrosoftIdentityWebAppAuthentication(configuration)
    .EnableTokenAcquisitionToCallDownstreamApi(initialScopes)
    .AddMicrosoftGraph("https://graph.microsoft.com/v1.0", initialScopes)
    .AddInMemoryTokenCaches();

services.AddControllersWithViews(options =>
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()));

services.AddRazorPages().AddMvcOptions(options =>
{
    //var policy = new AuthorizationPolicyBuilder()
    //    .RequireAuthenticatedUser()
    //    .Build();
    //options.Filters.Add(new AuthorizeFilter(policy));
}).AddMicrosoftIdentityUI();
```

## github actions build

Github actions is used for the DevOps. The build pipeline builds both the .NET project and the Angular nx project using npm. The two projects are built in the same step because the UI project is built into the wwwroot of the server project.

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

## Credits and used libraries

- NetEscapades.AspNetCore.SecurityHeaders
- Yarp.ReverseProxy
- Microsoft.Identity.Web
- ASP.NET Core
- Angular 
- Nx


## Links

https://learn.microsoft.com/en-us/aspnet/core/introduction-to-aspnet-core

https://nx.dev/getting-started/intro

https://github.com/AzureAD/microsoft-identity-web