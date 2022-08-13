# Dice Roller API

Ever wanted to toss 100 20-sided dice at once? Or some odd die, that doesn't roll well in reality? And resolve that with a JSON based API? Well then, here you go.

This is a simple Azure Function App I wrote to remember some .NET basics. It can either be run as a container or directly on your Azure Subscription. The base infrastructure can be deployed with the ARM template and the code itself can then be published using Visual Studio or the Azure Function Tools. Adjust and use as you like.

## Deployment Options

### Container

The Azure Functions Runtime [allows to build containers](https://docs.microsoft.com/en-us/azure/azure-functions/functions-create-function-linux-custom-image?tabs=in-process%2Cbash%2Cazure-cli&pivots=programming-language-csharp) and run them anywhere you see fit, like your local Docker Desktop environment, or even a Kubernetes Cluster.

#### Docker

Pull the container image from the [GitHub Container Registry](https://github.com/haseHH/azure-function-dice-api/pkgs/container/dice-roller-function) and start a container instance:

```bash
docker run -d -p 8080:80 --name dice-roller ghcr.io/hasehh/dice-roller-function:latest
```

Or build it yourself and run the container with that version:

```bash
docker build -t dice-roller-function:custom ./dice-roller/dice-roller/
docker run -d -p 8080:80 --name dice-roller dice-roller-function:custom
```

Remove the container when no longer needed:

```bash
docker rm dice-roller --force
```

#### Kubernetes

Take the [`dice-api.yml`](./kube/dice-api.yml), adjust it to match your setup and apply it to your cluster:

```bash
kubectl apply -f ./kube/dice-api.yml
```

### Azure Infrastructure

If you want to host the API in Azure, deploy the ARM template your favorite way or click the button below to deploy the resources in your own Azure Tenant:

[![Deploy to Azure](https://aka.ms/deploytoazurebutton)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2FhaseHH%2Fazure-function-dice-api%2Fmain%2Farm-templates%2Fazuredeploy.json)

The template contains six resources:

* The Function App itself
* An App Service Plan/Consumption Plan to run the app on
* A Storage Account to back the app
* An Application Insights resource for quick log insights
* A Log Analytics Workspace for deeper insights
* A Diagnostic Settings resource to cupple the app to the Log Analytics Workspace

## Calling the API

After deploying the app code to your resources or starting the container, you cann send HTTP GET requests to it. Or if you just want to make some tests (like, say, less than 20 requests per week) feel free to use my deployed instance. Either way, see the PowerShell code below for a few examples. You can request any (reasonable) number of many sided dice and add or subtract a modifier as needed.

```PowerShell
# Roll them all!
Invoke-RestMethod -Method GET -Uri 'http://localhost:8080/api/roll/100d20' -UseBasicParsing
Invoke-RestMethod -Method GET -Uri 'https://dice-api.hase.hamburg/api/roll/100d20' -UseBasicParsing

# Give me a realistic roll, and add some extra damage for my big axe.
Invoke-RestMethod -Method GET -Uri 'http://localhost:8080/api/roll/1d8+5' -UseBasicParsing
Invoke-RestMethod -Method GET -Uri 'https://dice-api.hase.hamburg/api/roll/1d8+5' -UseBasicParsing

# I angered my DM. Now my staff got split while blocking an attack and I get some damage deducted...
Invoke-RestMethod -Method GET -Uri 'http://localhost:8080/api/roll/2d4-2' -UseBasicParsing
Invoke-RestMethod -Method GET -Uri 'https://dice-api.hase.hamburg/api/roll/2d4-2' -UseBasicParsing
```

The API will then parse the requested dice from the end of the URI and roll up some random numbers for you. Example response:

```JSON
{
    "requestedToss": "2d12+2",
    "tosses": [
        10,
        3
    ],
    "result": 15
}
```

Currently all dice are rolled sequentially, so keep in mind that a request with too many dice may take some time.

## Licensing

This repository uses the [Unlicense](https://unlicense.org/) - that means it is part of the public domain and you can do whatever you want with the code.
