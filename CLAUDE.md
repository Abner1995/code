# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Repository Overview

This is a personal polyglot learning workspace. It contains multiple independent projects across C#, Python, and PHP. There is no top-level build system — each sub-project is self-contained.

## Projects

### C# / .NET

| Project | Path | Description | Has CLAUDE.md |
|---------|------|-------------|---------------|
| EShopMicroservices | `csharp/code/EShopMicroservices/` | .NET 8 microservices e-commerce system (CQRS, Marten, MassTransit, YARP, gRPC) | Yes |
| AvaViewUI | `csharp/code/AvaViewUI/` | Cross-platform Avalonia UI 11.2 app (Desktop/Browser/iOS/Android) | Yes |
| CleanArchitecture | `csharp/code/CleanArchitecture/` | ASP.NET Core 8 Web API with clean architecture layers | No |
| ContactSMS | `csharp/code/ContactSMS/` | .NET WebAPI learning project | No |
| Contact | `csharp/code/Contact/` | Clean architecture + MAUI contact app (.NET 8 + MySQL) | No |
| TodoMicroservices | `csharp/code/TodoMicroservices/` | Microservices todo app (ApiAdmin, ApiTodo, ApiUser, Ocelot gateway) | No |

**Shared conventions for C# projects:**
- All target .NET 8
- Build with `dotnet build` from the solution directory
- Run with `dotnet run --project <ProjectDir>`

### Python

| Project | Path | Description | Has CLAUDE.md |
|---------|------|-------------|---------------|
| stock | `python/stock/` | Chinese A-share stock data analysis (baostock, Elliott Wave) | Yes |

**Python environment:** Conda (`D:/code/python/stock/envs/stock310` on Windows, `/www/wwwroot/stock/envs/stock310` on Linux).

### PHP

| Path | Description |
|------|-------------|
| `php/code/` | Single-file PHP learning exercise (`1.php`) |

## Working with This Repository

- Each project in `csharp/code/` has its own `.sln` file. Open or build from that solution directory.
- Projects with their own `CLAUDE.md` contain detailed build commands, architecture notes, and conventions — read those when working within that project.
- The root `.gitignore` covers Visual Studio artifacts, build outputs, and common IDE files.
