# NuGet Package Deployment Guide

This guide walks you through the complete process of building, testing, and deploying your FluentNHibernate.DAL package to NuGet.org.

## 📋 Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) installed
- [NuGet.org](https://www.nuget.org) account created
- Project metadata configured in `.csproj` file
- All compilation warnings resolved

## 🧪 Step 3: Test Build Locally

Before deploying to NuGet.org, it's crucial to test your package build locally to ensure everything works correctly.

### Option A: Using PowerShell Script (Recommended)

```powershell
# Navigate to your project directory
cd C:\Path\To\FluentNHibernate.DAL

# Run local build test
.\deploy-nuget.ps1 -LocalOnly
```

### Option B: Using Batch Script

```cmd
# From Command Prompt
deploy-nuget.bat 1.0.0

# Or from PowerShell
cmd /c "deploy-nuget.bat 1.0.0"
```

### Option C: Manual Build Commands

```bash
# Clean previous builds
dotnet clean

# Restore packages
dotnet restore

# Build in Release mode
dotnet build --configuration Release

# Create NuGet package
dotnet pack --configuration Release --output ./nupkg
```

### 🔍 Verify Local Build

After successful build, check the following:

1. **Package File Created**: Look for `./nupkg/FluentNHibernate.DAL.{version}.nupkg`
2. **No Build Warnings**: Ensure clean build output
3. **Package Contents**: Use NuGet Package Explorer or extract to verify contents

```bash
# List package contents
dotnet nuget list source
```

### 🐛 Troubleshooting Local Build

| Issue                               | Solution                                            |
| ----------------------------------- | --------------------------------------------------- |
| Build fails with compilation errors | Fix code issues before packaging                    |
| Missing documentation files         | Ensure README.md and docs/ folder exist             |
| Package size too large              | Review included files and exclude unnecessary items |
| Dependency conflicts                | Update package references in `.csproj`              |

## 🚀 Step 4: Deploy to NuGet.org

Once your local build succeeds, you can deploy to NuGet.org.

### 🔑 4.1: Get Your NuGet API Key

1. **Sign in to NuGet.org**

   - Go to [nuget.org](https://www.nuget.org)
   - Sign in with your Microsoft account or create a new account

2. **Create API Key**

   - Navigate to your profile → **API Keys**
   - Click **Create** to generate a new API key
   - Set the following permissions:
     - **Key Name**: `FluentNHibernate.DAL-Deploy`
     - **Package Owner**: Your username
     - **Scopes**: `Push new packages and package versions`
     - **Packages**: `*` (or specify `FluentNHibernate.DAL`)
     - **Expiration**: Set appropriate expiration date

3. **Copy API Key**
   - ⚠️ **Important**: Copy the API key immediately as it won't be shown again
   - Store it securely (consider using environment variables)

### 🔧 4.2: Deploy Using Scripts

#### Option A: PowerShell Script Deployment

```powershell
# Deploy with inline API key
.\deploy-nuget.ps1 -Version "1.0.0" -ApiKey "YOUR_API_KEY_HERE"

# Or set environment variable first (more secure)
$env:NUGET_API_KEY = "YOUR_API_KEY_HERE"
.\deploy-nuget.ps1 -Version "1.0.0" -ApiKey $env:NUGET_API_KEY
```

#### Option B: Batch Script Deployment

```cmd
# From Command Prompt
deploy-nuget.bat 1.0.0 YOUR_API_KEY_HERE

# Or from PowerShell
cmd /c "deploy-nuget.bat 1.0.0 YOUR_API_KEY_HERE"
```

### 🛠️ 4.3: Manual Deployment

```bash
# Build package first
dotnet pack --configuration Release --output ./nupkg

# Push to NuGet.org
dotnet nuget push "./nupkg/FluentNHibernate.DAL.1.0.0.nupkg" \
  --api-key YOUR_API_KEY \
  --source https://api.nuget.org/v3/index.json
```

### 📊 4.4: Deployment Process

1. **Package Upload**: Your package is uploaded to NuGet.org
2. **Validation** (5-10 minutes): NuGet validates package format and security
3. **Indexing** (up to 30 minutes): Package becomes searchable
4. **Availability**: Users can install your package

### ✅ 4.5: Verify Deployment

1. **Check Package Page**: Visit `https://www.nuget.org/packages/FluentNHibernate.DAL`
2. **Test Installation**: Try installing in a test project
   ```bash
   dotnet add package FluentNHibernate.DAL
   ```
3. **Monitor Downloads**: Track usage in your NuGet.org dashboard

### 🚨 4.6: Deployment Troubleshooting

| Issue                 | Solution                                         |
| --------------------- | ------------------------------------------------ |
| `403 Forbidden`       | Check API key permissions and expiration         |
| `409 Conflict`        | Version already exists, increment version number |
| Package not appearing | Wait for indexing (up to 30 minutes)             |
| Validation errors     | Check package format and dependencies            |

## 🎨 Add Package Icon (Recommended)

A package icon makes your NuGet package more professional and recognizable.

### 📐 Icon Requirements

- **Format**: PNG
- **Size**: 128x128 pixels (minimum), 1024x1024 pixels (recommended)
- **Background**: Transparent or solid color
- **Content**: Simple, clear design that scales well

### 🎯 Icon Design Tips

1. **Keep it Simple**: Icons should be recognizable at small sizes
2. **Use Brand Colors**: Match your project's color scheme
3. **Avoid Text**: Icons should be symbolic, not text-based
4. **High Contrast**: Ensure visibility on both light and dark backgrounds

### 🛠️ Creating an Icon

#### Option A: Design Tools

- **Canva**: Use free templates for logo design
- **GIMP/Photoshop**: Create custom designs
- **Figma**: Web-based design tool
- **Icons8**: Icon generator with customization

#### Option B: Simple Text-Based Icon

```css
/* Example CSS for a simple icon */
Background: #2E86AB (Blue)
Text: "DAL" or "FNH"
Font: Bold, Sans-serif
Color: White
```

### 📁 Adding Icon to Project

1. **Save Icon**: Place `icon.png` in your project root

2. **Update Project File**: Edit `FluentNHibernate.DAL.csproj`

   ```xml
   <PropertyGroup>
     <!-- Uncomment this line -->
     <PackageIcon>icon.png</PackageIcon>
   </PropertyGroup>

   <!-- Add this if not present -->
   <ItemGroup>
     <None Include="icon.png" Pack="true" PackagePath="\" />
   </ItemGroup>
   ```

3. **Rebuild Package**: Run local build to test icon inclusion

   ```powershell
   .\deploy-nuget.ps1 -LocalOnly
   ```

4. **Deploy Updated Package**: Increment version and deploy
   ```xml
   <Version>1.0.1</Version>
   ```

### 🎭 Icon Examples

#### Simple Text Icons

```
┌─────────────┐    ┌─────────────┐    ┌─────────────┐
│             │    │             │    │             │
│     DAL     │    │     FNH     │    │     NH      │
│             │    │             │    │             │
└─────────────┘    └─────────────┘    └─────────────┘
```

#### Symbolic Icons

- Database symbol with gear (representing data access layer)
- Layered rectangles (representing architecture layers)
- Connected nodes (representing ORM relationships)

## 📈 Version Management

### 🔢 Versioning Strategy

Follow [Semantic Versioning](https://semver.org/):

- **MAJOR.MINOR.PATCH** (e.g., 1.0.0)
- **Major**: Breaking changes
- **Minor**: New features (backward compatible)
- **Patch**: Bug fixes (backward compatible)

### 📝 Release Notes

Update release notes in `.csproj` for each version:

```xml
<PackageReleaseNotes>
  v1.0.1 - Bug fixes and improvements
  - Fixed nullable reference warnings
  - Updated documentation
  - Improved error handling

  v1.0.0 - Initial release
  - Generic Repository Pattern
  - Code-First and Database-First support
  - Comprehensive documentation
</PackageReleaseNotes>
```

## 🔄 Continuous Deployment

### GitHub Actions Example

Create `.github/workflows/nuget-deploy.yml`:

```yaml
name: Deploy to NuGet

on:
  release:
    types: [published]

jobs:
  deploy:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3

      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: "8.0.x"

      - name: Restore dependencies
        run: dotnet restore

      - name: Build
        run: dotnet build --configuration Release --no-restore

      - name: Pack
        run: dotnet pack --configuration Release --no-build --output ./nupkg

      - name: Push to NuGet
        run: dotnet nuget push "./nupkg/*.nupkg" --api-key ${{secrets.NUGET_API_KEY}} --source https://api.nuget.org/v3/index.json
```

## 📚 Next Steps

After successful deployment:

1. **Update Documentation**: Add installation instructions
2. **Monitor Usage**: Track downloads and feedback
3. **Plan Updates**: Schedule regular maintenance releases
4. **Community Engagement**: Respond to issues and feature requests

## 🎯 Best Practices

- ✅ Always test locally before deploying
- ✅ Use semantic versioning consistently
- ✅ Include comprehensive documentation
- ✅ Respond to community feedback promptly
- ✅ Keep dependencies up to date
- ✅ Monitor security vulnerabilities
- ✅ Tag releases in source control

---

**Ready to deploy?** Follow these steps and your FluentNHibernate.DAL package will be available to the .NET community! 🚀
