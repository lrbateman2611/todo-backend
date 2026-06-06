# Development Environment Guide

This guide explains how to set up and use the development environment in Azure.

## Overview

The development environment consists of:

- **Separate Container App** (`todos-api-dev`) for testing
- **Automatic deployment** when creating a PR for the default branch
- **Lower resources** (0.25 CPU, 0.5GB RAM) to save costs
- **Shared infrastructure** (ACR, Key Vault, Database)

## Architecture

```
Production:  main branch → todos-api (0.5 CPU, 1GB RAM)
Development: dev branch    → todos-api-dev (0.25 CPU, 0.5GB RAM)
```

## Setup Instructions

### 1. Create Development Container App

Run the setup script:

```powershell
cd <path-to-your-todo-backend-repo>
.\.github\setup-dev-environment.ps1
```

This will:

- ✅ Create a new Container App for development
- ✅ Configure with lower resources
- ✅ Set ASPNETCORE_ENVIRONMENT=Development
- ✅ Provide the development URL

### 2. Configure Secrets

The development app requires the same secret names as production, but you must provide unique values:

**Option A: Azure Portal (Recommended)**

1. Go to Azure Portal → Container Apps → `todos-api-dev`
2. Click **Secrets** → **+ Add**
3. Add: `bitwarden-token` with your Bitwarden token
4. Go to **Containers** → **Edit and deploy**
5. Add environment variable:
   - Name: `Bitwarden__Token`
   - Source: **Reference a secret**
   - Value: `bitwarden-token`
6. Save and create

**Option B: Azure CLI**

```powershell
# Add Bitwarden secret
az containerapp secret set `
  --name todos-api-dev `
  --resource-group todos-containerapp-rg `
  --secrets bitwarden-token="YOUR_TOKEN"

# Configure environment variable
az containerapp update `
  --name todos-api-dev `
  --resource-group todos-containerapp-rg `
  --set-env-vars "Bitwarden__Token=secretref:bitwarden-token" "ASPNETCORE_ENVIRONMENT=Development"
```

### 3. Create Development Branch

```bash
# Create and checkout dev branch
git checkout -b dev

# Push to remote
git push -u origin dev
```

## Workflow

### Development Workflow

1. **Make changes** on `dev` branch

```bash
git checkout dev
# Make your changes...
git add .
git commit -m "Add new feature"
git push origin dev
```

2. **Automatic deployment** to `todos-api-dev`
   - GitHub Actions automatically deploys to development
   - Check https://github.com/lrbateman2611/todo-backend/actions

3. **Test your changes** on development URL

```powershell
# Get development URL
$devUrl = az containerapp show `
  --name todos-api-dev `
  --resource-group todos-containerapp-rg `
  --query properties.configuration.ingress.fqdn -o tsv

# Test endpoints
curl "https://$devUrl/health"
curl "https://$devUrl/"
```

4. **Merge to main** when ready for production

```bash
git checkout main
git merge dev
git push origin main
```

### Testing Pull Requests

Pull requests to `main` also trigger development deployment:

1. Create PR from `dev` to `main`
2. Development environment updates automatically
3. Test the changes
4. Merge PR when satisfied

## Environment URLs

| Environment | URL                                                         | Trigger                         |
| ----------- | ----------------------------------------------------------- | ------------------------------- |
| Production  | `todos-api.livelymeadow-*.eastus.azurecontainerapps.io`     | Push to `main`                |
| Development | `todos-api-dev.livelymeadow-*.eastus.azurecontainerapps.io` | Push to `dev` or PR to `main` |

## Resource Comparison

| Resource     | Production | Development | Purpose         |
| ------------ | ---------- | ----------- | --------------- |
| CPU          | 0.5 cores  | 0.25 cores  | Lower cost      |
| Memory       | 1 GB       | 0.5 GB      | Lower cost      |
| Min Replicas | 1          | 1           | Always running  |
| Max Replicas | 10         | 3           | Limited scaling |
| Environment  | Production | Development | Feature flags   |

## Cost Optimization

Development environment costs ~50% less than production:

- **Lower CPU/Memory**: 0.25 CPU vs 0.5 CPU
- **Lower scaling**: Max 3 replicas vs 10
- **Shared resources**: Same ACR, Key Vault

Estimated costs:

- Production: ~$15-30/month
- Development: ~$7-15/month

### Optional: Stop Development When Not Needed

```powershell
# Stop development app to save costs
az containerapp update `
  --name todos-api-dev `
  --resource-group todos-containerapp-rg `
  --min-replicas 0 `
  --max-replicas 0

# Start it again when needed
az containerapp update `
  --name todos-api-dev `
  --resource-group todos-containerapp-rg `
  --min-replicas 1 `
  --max-replicas 3
```

## Monitoring

### View Logs

```powershell
# Development logs
az containerapp logs show `
  --name todos-api-dev `
  --resource-group todos-containerapp-rg `
  --tail 100 `
  --follow
```

### Check Deployment Status

Go to GitHub Actions:

- https://github.com/lrbateman2611/todo-backend/actions

Look for:

- **"Build and Deploy to Azure Container Apps"** (Production)
- **"Deploy to Development"** (Development)

## Troubleshooting

### Development app not updating

```powershell
# Force restart
az containerapp revision restart `
  --name todos-api-dev `
  --resource-group todos-containerapp-rg `
  --revision todos-api-dev--[revision-name]
```

### Check current image

```powershell
az containerapp show `
  --name todos-api-dev `
  --resource-group todos-containerapp-rg `
  --query properties.template.containers[0].image
```

### Rollback development

```powershell
# List dev images
az acr repository show-tags --name todosacr3863 --repository todos-backend --orderby time_desc | Select-String "dev"

# Rollback to specific version
az containerapp update `
  --name todos-api-dev `
  --resource-group todos-containerapp-rg `
  --image todosacr3863.azurecr.io/todos-backend:dev-COMMIT_SHA
```

## Best Practices

1. ✅ **Always test in dev first** before merging to main
2. ✅ **Use feature branches** that merge to `dev`
3. ✅ **Keep dev environment running** during active development
4. ✅ **Stop dev when not needed** to save costs
5. ✅ **Use different databases** if possible (not sharing production data)
6. ✅ **Review logs** in development before promoting to production

## Advanced: Separate Databases

For complete isolation, consider:

1. **Separate Supabase project** for development
2. **Separate secrets** in Key Vault
3. **Different Auth0 tenant** for development

This prevents development from affecting production data.

## Quick Commands

```powershell
# Switch to development
git checkout dev

# Make changes and deploy to dev
git add . && git commit -m "Feature" && git push origin dev

# Promote to production
git checkout main
git merge dev
git push origin main

# View dev app
az containerapp show --name todos-api-dev --resource-group todos-containerapp-rg

# View prod app
az containerapp show --name todos-api --resource-group todos-containerapp-rg
```

## Summary

| Action                 | Command/Location                      |
| ---------------------- | ------------------------------------- |
| Create dev environment | `.\.github\setup-dev-environment.ps1` |
| Deploy to dev          | `git push origin dev`                 |
| View dev URL           | Portal or `az containerapp show`      |
| Test dev               | `curl https://DEV_URL/health`         |
| Promote to prod        | Merge `dev` → `main`                |
| View deployments       | GitHub Actions tab                    |

## Support

For issues with:

- **Development setup**: Check `.github/setup-dev-environment.ps1`
- **Deployment workflow**: Check `.github/workflows/deploy-dev.yml`
- **Production deployment**: Check `.github/workflows/deploy-azure.yml`
