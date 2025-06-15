@echo off
REM FluentNHibernate.DAL NuGet Package Deployment Script
REM Usage: deploy-nuget.bat [version] [api-key]

set VERSION=%1
set API_KEY=%2

if "%VERSION%"=="" set VERSION=1.0.0

echo === FluentNHibernate.DAL NuGet Deployment ===
echo Version: %VERSION%

echo Cleaning previous builds...
if exist bin rmdir /s /q bin
if exist obj rmdir /s /q obj
if exist nupkg rmdir /s /q nupkg

echo Restoring NuGet packages...
dotnet restore
if errorlevel 1 (
    echo Package restore failed
    exit /b 1
)

echo Building project...
dotnet build --configuration Release --no-restore
if errorlevel 1 (
    echo Build failed
    exit /b 1
)

echo Creating NuGet package...
dotnet pack --configuration Release --no-build --output ./nupkg
if errorlevel 1 (
    echo Package creation failed
    exit /b 1
)

echo Package created successfully!
dir .\nupkg\*.nupkg

if "%API_KEY%"=="" (
    echo.
    echo No API key provided. To deploy manually, run:
    echo   dotnet nuget push "./nupkg/FluentNHibernate.DAL.%VERSION%.nupkg" --api-key YOUR_API_KEY --source nuget.org
    echo.
    echo Or run: deploy-nuget.bat %VERSION% YOUR_API_KEY
    goto :end
)

echo Deploying to NuGet.org...
dotnet nuget push "./nupkg/FluentNHibernate.DAL.%VERSION%.nupkg" --api-key %API_KEY% --source nuget.org

if errorlevel 1 (
    echo Package deployment failed
    exit /b 1
) else (
    echo Package deployed successfully!
    echo It may take a few minutes to appear on nuget.org
)

:end
echo === Deployment Complete ===
pause 