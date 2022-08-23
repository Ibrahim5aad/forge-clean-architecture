# Forge Sample - Clean Architecture (.NET Core)

![.NET](https://img.shields.io/badge/.NET%20Core-3.1-blue.svg)

[![oAuth2](https://img.shields.io/badge/oAuth2-v1-green.svg)](http://developer.autodesk.com/)
[![OSS](https://img.shields.io/badge/OSS-v2-green.svg)](http://developer.autodesk.com/)
[![Model-Derivative](https://img.shields.io/badge/Model%20Derivative-v2-green.svg)](http://developer.autodesk.com/)
[![Viewer](https://img.shields.io/badge/Viewer-v6-green.svg)](http://developer.autodesk.com/)

![Intermediate](https://img.shields.io/badge/Level-Intermediate-blue.svg)

# Description

This basic .NET Core WebAPI sample implements a basic list of Buckets and Objects with an [Autodesk Forge 2 Legged OAuth](https://developer.autodesk.com/en/docs/oauth/v2/tutorials/get-2-legged-token/). 

[3 Legged Authentication](https://forge.autodesk.com/en/docs/oauth/v2/tutorials/get-3-legged-token/) is also available, you can switch between
the two OAuth services.

The front-end was desiged with pure HTML + JavaScript (jQuery, Bootstrap). It includes: create and delete buckets, upload, delete and translate object (including `.zip` files). This code is based on [this tutorial](http://learnforge.autodesk.io).
 
# Setup

## Prerequisites

1. **Forge Account**: Learn how to create a Forge Account, activate subscription and create an app at [this tutorial](http://learnforge.autodesk.io/#/account/). 
2. **Visual Studio**: Either Community 2017+ (Windows) or Code (Windows, MacOS).
3. **.NET Core** basic knowledge with C#
4. **JavaScript** basic knowledge with **jQuery**
5. **ngrok**: Routing tool, [download here](https://ngrok.com/)

## Running locally

Clone this project: git clone github.com/Autodesk-Forge/model.derivative-WebAPI-sample.git

**Visual Studio** (Windows):

Right-click on the project, then go to **Debug**. Adjust the settings as shown below. 

![](forgeSample/wwwroot/img/readme/visual_studio_settings.png)

**Visual Sutdio Code** (Windows, MacOS):

Open the folder, at the bottom-right, select **Yes** and **Restore**. This restores the packages (e.g. Autodesk.Forge) and creates the launch.json file. See *Tips & Tricks* for .NET Core on MacOS.

![](forgeSample/wwwroot/img/readme/visual_code_restore.png)

At the `.vscode\launch.json`, find the env vars and add your Forge Client ID, Secret and callback URL. Also define the `ASPNETCORE_URLS` variable. The end result should be as shown below:

```json
"env": {
    "ASPNETCORE_ENVIRONMENT": "Development",
    "ASPNETCORE_URLS" : "http://localhost:3000",
    "FORGE_CLIENT_ID": "your id here",
    "FORGE_CLIENT_SECRET": "your secret here",
    "FORGE_WEBHOOK_URL": "your ngrok address here: e.g. http://abcd1234.ngrok.io",
},
```

Run `ngrok http 3000` to create a tunnel to your local machine, then copy the address into the `FORGE_WEBHOOK_URL` environment variable.Ngrok is a cross-platform application that exposes local server ports to the Internet. 
 
# Further Reading

Documentation:

- [Data Management API](https://developer.autodesk.com/en/docs/data/v2/overview/)
- [Model Derivative API](https://forge.autodesk.com/en/docs/model-derivative/v2/developers_guide/overview/)
- [Viewer](https://developer.autodesk.com/en/docs/viewer/v6)
 
 
