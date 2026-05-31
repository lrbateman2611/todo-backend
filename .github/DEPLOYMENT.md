# GitHub Actions Azure Deployment Setup

This repository uses GitHub Actions to automatically build and deploy to Azure Container Apps when code is pushed to the `master` branch.

## Prerequisites

- Azure subscription
- Azure Container Registry (ACR)
- Azure Container Apps instance
- GitHub repository

## Setup Instructions

### 1. Create Azure Service Principal

Run this command in PowerShell or Azure Cloud Shell:

```powershell
# Set your resource group and subscription
$resourceGroup = "todos-containerapp-rg"
$subscriptionId = (az account show --query id -o tsv)

# Create service principal with Contributor role on the resource group
$sp = az ad sp create-for-rbac `
  --name "github-actions-todos-deploy" `
  --role contributor `
  --scopes "/subscriptions/$subscriptionId/resourceGroups/$resourceGroup" `
  --sdk-auth

# Output the credentials (copy this JSON)
Write-Host $sp -ForegroundColor Green
```

The output will look like this:
```json
{
  "clientId": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
  "clientSecret": "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
  "subscriptionId": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
  "tenantId": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
  "activeDirectoryEndpointUrl": "https://login.microsoftonline.com",
  "resourceManagerEndpointUrl": "https://management.azure.com/",
  "activeDirectoryGraphResourceId": "https://graph.windows.net/",
  "sqlManagementEndpointUrl": "https://management.core.windows.net:8443/",
  "galleryEndpointUrl": "https://gallery.azure.com/",
  "managementEndpointUrl": "https://management.core.windows.net/"
}
```

### 2. Add Secret to GitHub

1. Go to your GitHub repository
2. Navigate to **Settings** → **Secrets and variables** → **Actions**
3. Click **New repository secret**
4. Name: `AZURE_CREDENTIALS`
5. Value: Paste the entire JSON output from step 1
6. Click **Add secret**

### 3. Verify Environment Variables

In the workflow file (`.github/workflows/deploy-azure.yml`), verify these match your Azure resources:

```yaml
env:
  AZURE_RESOURCE_GROUP: todos-containerapp-rg
  CONTAINER_APP_NAME: todos-api
  ACR_NAME: todosacr3863
  IMAGE_NAME: todos-backend
```

Update them if your resource names are different.

### 4. Grant Service Principal ACR Access

The service principal needs permission to push to ACR:

```powershell
# Get the service principal app ID
$spAppId = az ad sp list --display-name "github-actions-todos-deploy" --query "[0].appId" -o tsv

# Get ACR resource ID
$acrId = az acr show --name todosacr3863 --resource-group todos-containerapp-rg --query id -o tsv

# Grant AcrPush role
az role assignment create `
  --assignee $spAppId `
  --role AcrPush `
  --scope $acrId

Write-Host "Service Principal granted ACR push access" -ForegroundColor Green
```

## Workflow Details

### Trigger
- **Push to `master` branch**: Automatically builds and deploys
- **Manual trigger**: Can be run manually from GitHub Actions tab

### Steps
1. ✅ Checkout code
2. ✅ Login to Azure
3. ✅ Build Docker image
4. ✅ Push to Azure Container Registry with commit SHA and `latest` tags
5. ✅ Update Container App with new image
6. ✅ Display deployment summary

### Image Tags
- `latest` - Always points to the most recent build
- `<commit-sha>` - Specific version for rollback capability

## Testing the Workflow

### Option 1: Push to Master
```bash
git add .
git commit -m "Test deployment workflow"
git push origin master
```

### Option 2: Manual Trigger
1. Go to GitHub → **Actions** tab
2. Select **Build and Deploy to Azure Container Apps**
3. Click **Run workflow**
4. Select `master` branch
5. Click **Run workflow**

## Monitoring Deployment

1. Go to GitHub → **Actions** tab
2. Click on the running workflow
3. Expand steps to see detailed logs
4. Check the deployment summary at the bottom

## Rollback to Previous Version

If you need to rollback to a previous version:

```powershell
# List available image tags
az acr repository show-tags --name todosacr3863 --repository todos-backend --orderby time_desc

# Rollback to specific commit SHA
az containerapp update `
  --name todos-api `
  --resource-group todos-containerapp-rg `
  --image todosacr3863.azurecr.io/todos-backend:<commit-sha>
```

## Troubleshooting

### Error: "Failed to authenticate"
- Verify `AZURE_CREDENTIALS` secret is set correctly in GitHub
- Ensure service principal has Contributor role on resource group

### Error: "ACR login failed"
- Verify service principal has `AcrPush` role on ACR
- Check ACR name is correct in workflow

### Error: "Container App not found"
- Verify resource group and container app name in workflow
- Ensure service principal has access to the resource group

## Security Best Practices

✅ Service principal scoped to specific resource group only  
✅ Secrets stored in GitHub Secrets (encrypted)  
✅ Images tagged with commit SHA for traceability  
✅ Minimal permissions (Contributor on resource group, AcrPush on ACR)  

## Additional Resources

- [Azure Container Apps Documentation](https://learn.microsoft.com/azure/container-apps/)
- [GitHub Actions for Azure](https://github.com/Azure/actions)
- [Docker Best Practices](https://docs.docker.com/develop/dev-best-practices/)
