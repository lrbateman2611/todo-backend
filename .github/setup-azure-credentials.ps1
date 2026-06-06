# Quick Setup Script for GitHub Actions Azure Deployment
# Run this script to set up the service principal and get the credentials

param(
	[Parameter(Mandatory=$false)]
	[string]$ResourceGroup = "todos-containerapp-rg",

	[Parameter(Mandatory=$false)]
	[string]$AcrName = "todosacr3863",

	[Parameter(Mandatory=$false)]
	[string]$ServicePrincipalName = "github-actions-todos-deploy"
)

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "GitHub Actions Azure Deployment Setup" -ForegroundColor Cyan
Write-Host "========================================`n" -ForegroundColor Cyan

# Get subscription ID
Write-Host "Getting Azure subscription..." -ForegroundColor Yellow
$subscriptionId = az account show --query id -o tsv
$subscriptionName = az account show --query name -o tsv

Write-Host "✓ Subscription: $subscriptionName" -ForegroundColor Green
Write-Host "✓ Subscription ID: $subscriptionId`n" -ForegroundColor Green

# Create service principal
Write-Host "Creating service principal..." -ForegroundColor Yellow
$sp = az ad sp create-for-rbac `
	--name $ServicePrincipalName `
	--role contributor `
	--scopes "/subscriptions/$subscriptionId/resourceGroups/$ResourceGroup" `
	--sdk-auth

if ($LASTEXITCODE -ne 0) {
	Write-Host "✗ Failed to create service principal" -ForegroundColor Red
	Write-Host "It might already exist. Checking..." -ForegroundColor Yellow

	$spAppId = az ad sp list --display-name $ServicePrincipalName --query "[0].appId" -o tsv

	if ($spAppId) {
		Write-Host "✓ Service principal exists: $ServicePrincipalName" -ForegroundColor Green
		Write-Host "⚠ You'll need to reset credentials or use existing ones" -ForegroundColor Yellow
	}
} else {
	Write-Host "✓ Service principal created successfully`n" -ForegroundColor Green
}

# Get service principal app ID
$spAppId = az ad sp list --display-name $ServicePrincipalName --query "[0].appId" -o tsv

# Grant ACR push access
Write-Host "Granting ACR push access..." -ForegroundColor Yellow
$acrId = az acr show --name $AcrName --resource-group $ResourceGroup --query id -o tsv

az role assignment create `
	--assignee $spAppId `
	--role AcrPush `
	--scope $acrId `
	2>$null

if ($LASTEXITCODE -eq 0) {
	Write-Host "✓ ACR push access granted`n" -ForegroundColor Green
} else {
	Write-Host "⚠ ACR role assignment might already exist`n" -ForegroundColor Yellow
}

# Display credentials
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "AZURE_CREDENTIALS Secret Value" -ForegroundColor Cyan
Write-Host "========================================`n" -ForegroundColor Cyan

Write-Host $sp -ForegroundColor Green

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "Next Steps" -ForegroundColor Cyan
Write-Host "========================================`n" -ForegroundColor Cyan

Write-Host "1. Copy the JSON above (entire block)" -ForegroundColor Yellow
Write-Host "2. Go to GitHub → Settings → Secrets and variables → Actions" -ForegroundColor Yellow
Write-Host "3. Click 'New repository secret'" -ForegroundColor Yellow
Write-Host "4. Name: " -NoNewline -ForegroundColor Yellow
Write-Host "AZURE_CREDENTIALS" -ForegroundColor White
Write-Host "5. Value: " -NoNewline -ForegroundColor Yellow
Write-Host "Paste the JSON" -ForegroundColor White
Write-Host "6. Click 'Add secret'" -ForegroundColor Yellow
Write-Host "7. Push code to main branch to trigger deployment`n" -ForegroundColor Yellow

Write-Host "✓ Setup complete!" -ForegroundColor Green
