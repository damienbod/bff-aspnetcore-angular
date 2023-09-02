# BFF security architecture using ASP.NET core and nx Angular

## Setup Server 

In production, the Angular nx project is built into the wwwroot of the .NET project.

Configure the YARP reverse proxy to match the Angular nx URL. This is only required in development.

```json
 "UiDevServerUrl": "https://localhost:4201",
  "ReverseProxy": {
    "Routes": {
      "route1": {
        "ClusterId": "cluster1",
        // "AuthorizationPolicy": "CookieAuthenticationPolicy",
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

## Setup development

We always run in HTTPS

```json
nx server --ssl
```

## Azure App Registration setup

The application(s) are deployed as one. 

This is an OpenID Connect confidential client with a user secret or a certification for client assertion.

Use the Web client type on setup.

## github actions build

## github actions Azure deployment