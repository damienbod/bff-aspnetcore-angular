# BFF security architecture using ASP.NET Core and nx Angular standalone

[![.NET and npm build](https://github.com/damienbod/bff-aspnetcore-angular/actions/workflows/dotnet.yml/badge.svg)](https://github.com/damienbod/bff-aspnetcore-angular/actions/workflows/dotnet.yml) [![Build and deploy to Azure Web App](https://github.com/damienbod/bff-aspnetcore-angular/actions/workflows/azure-webapps-dotnet-core.yml/badge.svg?branch=deploy)](https://github.com/damienbod/bff-aspnetcore-angular/actions/workflows/azure-webapps-dotnet-core.yml) [![License](https://img.shields.io/badge/license-Apache%20License%202.0-blue.svg)](https://github.com/damienbod/bff-aspnetcore-angular/blob/main/LICENSE)

## Setup Server

The ASP.NET Core project is setup to run in development and production. In production, it uses the Angular production build deployed to the wwwroot. In development, it uses MS YARP reverse proxy to forward requests.

> [!IMPORTANT]  
> In production, the Angular nx project is built into the **wwwroot** of the .NET project.

![BFF production](https://github.com/damienbod/bff-aspnetcore-angular/blob/main/images/bff-arch-production_01.png)

Configure the YARP reverse proxy to match the Angular nx URL. This is only required in development. I always use HTTPS in development and the port needs to match the Angular nx developement env.

> [!IMPORTANT]  
> In a real Angular project, the additional dev routes need to be added so that the __dev refresh__ works!

```json
 "UiDevServerUrl": "https://localhost:4201",
 "ReverseProxy": {
    "Routes": {
      "assets": {
        "ClusterId": "cluster1",
        "Match": {
          "Path": "assets/{**catch-all}"
        }
      },
      "routealljs": {
        "ClusterId": "cluster1",
        "Match": {
          "Path": "{nomatterwhat}.js"
        }
      },
      "routeallcss": {
        "ClusterId": "cluster1",
        "Match": {
          "Path": "{nomatterwhat}.css"
        }
      },
      "webpacklazyloadingsources": {
        "ClusterId": "cluster1",
        "Match": {
          "Path": "/src_{nomatterwhat}_ts.js"
        }
      },
      "signalr": {
        "ClusterId": "cluster1",
        "Match": {
          "Path": "/ng-cli-ws"
        }
      },
      "webpacknodesrcmap": {
        "ClusterId": "cluster1",
        "Match": {
          "Path": "/{nomatterwhat}.js.map"
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
> The default Angular setup uses port 4200, this needs to match the YARP reverse proxy settings for development.

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

The development environment is setup to use the default tools for each of the tech stacks. Angular nx is used like recommended. I use Visual Studio code. A YARP reverse proxy is used to integrate the Angular development into the backend application.

![BFF development](https://github.com/damienbod/bff-aspnetcore-angular/blob/main/images/bff-arch-development_01.png)

> [!NOTE]  
> Always run in HTTPS, both in development and production

```
nx serve --ssl
```

## Azure App Registration setup

The application(s) are deployed as one. This is an OpenID Connect confidential client with a user secret or a certification for client assertion.

Use the Web client type on setup.

![BFF Azure registration](https://github.com/damienbod/bff-aspnetcore-angular/blob/main/images/azure-app-registration_01.png)

The OpenID Connect client is setup using **Microsoft.Identity.Web**. This implements the Microsoft Entra ID client. I have created downstream APIs using the OBO flow and a Microsoft Graph client. This could be replaced with any OpenID Connect client and requires no changes in the frontend part of the solution.

```csharp
var scopes = configuration.GetValue<string>("DownstreamApi:Scopes");
string[] initialScopes = scopes!.Split(' ');

services.AddMicrosoftIdentityWebAppAuthentication(configuration, "MicrosoftEntraID")
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

Add the Azure App registration settings to the **appsettings.Development.json** and the **ClientSecret** to the user secrets.

```json
"MicrosoftEntraID": {
    "Instance": "https://login.microsoftonline.com/",
    "Domain": "[Enter the domain of your tenant, e.g. contoso.onmicrosoft.com]",
    "TenantId": "[Enter 'common', or 'organizations' or the Tenant Id (Obtained from the Azure portal. Select 'Endpoints' from the 'App registrations' blade and use the GUID in any of the URLs), e.g. da41245a5-11b3-996c-00a8-4d99re19f292]",
    "ClientId": "[Enter the Client Id (Application ID obtained from the Azure portal), e.g. ba74781c2-53c2-442a-97c2-3d60re42f403]",
    "ClientSecret": "[Copy the client secret added to the app from the Azure portal]",
    "ClientCertificates": [
    ],
    // the following is required to handle Continuous Access Evaluation challenges
    "ClientCapabilities": [ "cp1" ],
    "CallbackPath": "/signin-oidc"
},
```

App Service (linux plan) configuration 

```
MicrosoftEntraID__Instance               --your-value--
MicrosoftEntraID__Domain                 --your-value--
MicrosoftEntraID__TenantId               --your-value--
MicrosoftEntraID__ClientId               --your-value--
MicrosoftEntraID__CallbackPath           /signin-oidc
MicrosoftEntraID__SignedOutCallbackPath  /signout-callback-oidc
```

The client secret or client certificate needs to be setup, see Microsoft Entra ID documentation.

## Debugging

Start the Angular project from the **ui** folder

```
nx serve --ssl
```

Start the ASP.NET Core project from the **server** folder

```
dotnet run
```

Or just open Visual Studio and run the solution.

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
          dotnet-version: 8.0.x

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

The deployment pipeline builds both projects and deploys this to Azure using an Azure App Service. See **azure-webapps-dotnet-core.yml**

deployment test server: https://bff-angular-aspnetcore.azurewebsites.net

## Credits and used libraries

- NetEscapades.AspNetCore.SecurityHeaders
- Yarp.ReverseProxy
- Microsoft.Identity.Web
- ASP.NET Core
- Angular 
- Nx

## Angular nx Updates

```
nx migrate latest

nx migrate --run-migrations=migrations.json
```

## Links

- [SonarQube Cloud - Analyzing GitHub projects](https://docs.sonarsource.com/sonarcloud/getting-started/github/)
- [rufer7 - github-sonarcloud-integration](https://github.com/rufer7/github-sonarcloud-integration)
- [[HOWTO] Integrate SonarCloud analysis in an Azure DevOps YAML pipeline](https://blog.rufer.be/2023/10/06/howto-integrate-sonarcloud-analysis-in-an-azure-devops-yaml-pipeline/)
- [Sonar Community - Code coverage report for .Net not working on Linux agent](https://community.sonarsource.com/t/code-coverage-report-for-net-not-working-on-linux-agent/62087)
- [SonarScanner for .NET - Analyzing languages other than C# and VB](https://docs.sonarsource.com/sonarcloud/advanced-setup/ci-based-analysis/sonarscanner-for-net/#analyzing-languages-other-than-c-and-vb)
- [Andrei Epure - How to analyze JS/TS, HTML and CSS files with the Sonar Scanner for .NET](https://andreiepure.ro/2023/08/20/analyze-web-files-with-s4net.html)
- [damienbod - bff-aspnetcore-angular](https://github.com/damienbod/bff-aspnetcore-angular)
- [[Webinar] End-to-end security in a web application](https://community.sonarsource.com/t/webinar-end-to-end-security-in-a-web-application/115405)
