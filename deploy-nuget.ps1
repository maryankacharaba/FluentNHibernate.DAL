# FluentNHibernate.DAL NuGet Package Deployment Script
# 
# This script builds and deploys the NuGet package
# Make sure you have your NuGet API key configured first

param(
    [Parameter(Mandatory = $false)]
    [string]$Version = "1.0.0",
    
    [Parameter(Mandatory = $false)]
    [string]$ApiKey = "",
    
    [Parameter(Mandatory = $false)]
    [switch]$LocalOnly = $false
)

Write-Host "=== FluentNHibernate.DAL NuGet Deployment ===" -ForegroundColor Green
Write-Host "Version: $Version" -ForegroundColor Yellow

# Clean previous builds
Write-Host "Cleaning previous builds..." -ForegroundColor Blue
Remove-Item "bin" -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item "obj" -Recurse -Force -ErrorAction SilentlyContinue

# Restore packages
Write-Host "Restoring NuGet packages..." -ForegroundColor Blue
dotnet restore
if ($LASTEXITCODE -ne 0) {
    Write-Error "Package restore failed"
    exit 1
}

# Build the project
Write-Host "Building project..." -ForegroundColor Blue
dotnet build --configuration Release --no-restore
if ($LASTEXITCODE -ne 0) {
    Write-Error "Build failed"
    exit 1
}

# Create NuGet package
Write-Host "Creating NuGet package..." -ForegroundColor Blue
dotnet pack --configuration Release --no-build --output ./nupkg
if ($LASTEXITCODE -ne 0) {
    Write-Error "Package creation failed"
    exit 1
}

# List created packages
Write-Host "Created packages:" -ForegroundColor Green
Get-ChildItem "./nupkg/*.nupkg" | ForEach-Object { Write-Host "  - $($_.Name)" -ForegroundColor Yellow }

if ($LocalOnly) {
    Write-Host "Local build completed successfully!" -ForegroundColor Green
    Write-Host "Package location: ./nupkg/" -ForegroundColor Yellow
    exit 0
}

# Deploy to NuGet.org
if ([string]::IsNullOrEmpty($ApiKey)) {
    Write-Host "No API key provided. Use -ApiKey parameter or set NUGET_API_KEY environment variable" -ForegroundColor Red
    Write-Host "To deploy manually, run:" -ForegroundColor Yellow
    Write-Host "  dotnet nuget push './nupkg/FluentNHibernate.DAL.$Version.nupkg' --api-key YOUR_API_KEY --source nuget.org" -ForegroundColor Cyan
    exit 1
}

Write-Host "Deploying to NuGet.org..." -ForegroundColor Blue
$packagePath = "./nupkg/FluentNHibernate.DAL.$Version.nupkg"
dotnet nuget push $packagePath --api-key $ApiKey --source nuget.org

if ($LASTEXITCODE -eq 0) {
    Write-Host "Package deployed successfully!" -ForegroundColor Green
    Write-Host "It may take a few minutes to appear on nuget.org" -ForegroundColor Yellow
}
else {
    Write-Error "Package deployment failed"
    exit 1
}

Write-Host "=== Deployment Complete ===" -ForegroundColor Green 