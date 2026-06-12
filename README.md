# DeploymentTool

A powerful .NET 8 console application for automated deployment of .NET web applications with built-in rollback, IIS management, and pipeline orchestration.

![.NET Version](https://img.shields.io/badge/.NET-8.0-purple)
![Version](https://img.shields.io/badge/version-2.4-blue)
![License](https://img.shields.io/badge/license-MIT-green)

## Overview

DeploymentTool is a comprehensive deployment automation solution designed to streamline the deployment of .NET web applications to local or remote environments. It features an interactive console UI, configuration-driven pipelines, and robust error handling with automatic rollback capabilities.

## Key Features

- 🔄 **Automated Deployment Pipelines** - Configure and execute multi-step deployment pipelines via JSON configuration files
- 🔙 **Rollback Support** - Automatic backup creation and rollback on deployment failures
- 🌐 **IIS Integration** - Built-in IIS management including app pool control and site management
- 📦 **MSBuild Integration** - Automated project building and publishing using .NET CLI
- 🔀 **Git Integration** - Optional Git operations (stash, pull, branch validation) as part of the deployment workflow
- 🎯 **Environment Deployments** - Deploy multiple pipelines in sequence with automatic rollback on any failure
- 💻 **Interactive Mode** - Menu-driven interface for selecting and executing deployments
- 📊 **Flexible Logging** - Support for file-based or SQL logging of deployment activities
- ⚙️ **Highly Configurable** - JSON-based configuration for all deployment settings including source, target, credentials, and pipeline stages

## Deployment Modes

- **Local Deployment** - Deploy to local file system with optional IIS management
- **Pipeline Deployment** - Single-configuration deployments
- **Environment Deployment** - Multi-pipeline orchestration with coordinated rollback

## Technologies Used

- .NET 8.0
- Microsoft.Build for project compilation
- Microsoft.Web.Administration for IIS management
- RazorConsole.Core for interactive UI
- Newtonsoft.Json for configuration management
- Cake.Powershell for PowerShell integration

## Requirements

- ⚠️ **Must be run with Administrator privileges**
- Windows operating system (for IIS integration)
- .NET 8.0 Runtime or SDK
