# Azure Development Environment Setup Script
# This script creates a development Container App environment

param(
	[Parameter(Mandatory=$false)]
	[string]$ResourceGroup = "todos-containerapp-rg",

	[Parameter(Mandatory=$false)]
	[string]$Location = "eastus",

	[Parameter(Mandatory=$false)]
	[string]$AcrName = "todosacr3863",

	[Parameter(Mandatory=$false)]
	[string]$ContainerAppEnv = "todos-env",

	[Parameter(Mandatory=$false)]
	[string]$DevAppName = "todos-api-dev"
)

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Creating Development Environment" -ForegroundColor Cyan
Write-Host "========================================`n" -ForegroundColor Cyan

# Get ACR details
Write-Host "Getting ACR information..." -ForegroundColor Yellow
$loginServer = az acr show --name $AcrName --query loginServer -o tsv
Write-Host "✓ ACR: $loginServer`n" -ForegroundColor Green

# Check if Container App already exists
Write-Host "Checking if development Container App exists..." -ForegroundColor Yellow
$appExists = az containerapp show --name $DevAppName --resource-group $ResourceGroup 2>$null

if ($appExists) {
	Write-Host "⚠ Development Container App already exists: $DevAppName" -ForegroundColor Yellow
	Write-Host "Delete it to recreate it, or skip to leave it unchanged.`n" -ForegroundColor Yellow

	$action = Read-Host "Do you want to (d)elete/recreate it or (s)kip it? [d/s]"

	if ($action -eq 'd') {
		Write-Host "`nDeleting existing Container App..." -ForegroundColor Yellow
		az containerapp delete --name $DevAppName --resource-group $ResourceGroup --yes
		Write-Host "✓ Deleted`n" -ForegroundColor Green
		$appExists = $null
	}
}

if (-not $appExists) {
	Write-Host "Creating development Container App..." -ForegroundColor Yellow

	# Create development Container App with system-assigned managed identity
	az containerapp create `
		--name $DevAppName `
		--resource-group $ResourceGroup `
		--environment $ContainerAppEnv `
		--image "$loginServer/todos-backend:latest" `
		--target-port 8080 `
		--ingress external `
		--registry-server $loginServer `
		--system-assigned `
		--min-replicas 1 `
		--max-replicas 3 `
		--cpu 0.25 `
		--memory 0.5Gi `
		--env-vars "ASPNETCORE_ENVIRONMENT=Development"

	Write-Host "✓ Development Container App created`n" -ForegroundColor Green

	# Get the managed identity's principal ID
	Write-Host "Configuring managed identity for ACR access..." -ForegroundColor Yellow
	
	# Wait for the managed identity to be created
	$principalId = $null
	$retryCount = 0
	$maxRetries = 10
	$lastError = $null
	
	while ($retryCount -lt $maxRetries) {
		# Try to get the managed identity, capturing both stdout and stderr
		$output = az containerapp show --name $DevAppName --resource-group $ResourceGroup --query identity.principalId -o tsv 2>&1
		if (-not [string]::IsNullOrWhiteSpace($output) -and $output -notlike "*error*") {
			$principalId = $output
			break
		}
		
		$lastError = $output
		$retryCount++
		if ($retryCount -lt $maxRetries) {
			Write-Host "Waiting for managed identity to be available... (attempt $retryCount/$maxRetries)" -ForegroundColor Gray
			Start-Sleep -Seconds 2
		}
	}
	
	if ([string]::IsNullOrWhiteSpace($principalId)) {
		Write-Host "✗ Failed to retrieve managed identity for Container App: $DevAppName" -ForegroundColor Red
		if ($lastError) {
			Write-Host "Error details: $lastError" -ForegroundColor Gray
		}
		Write-Host "Please manually assign AcrPull role to the Container App's managed identity." -ForegroundColor Yellow
		exit 1
	}

	# Get ACR resource ID - separate stdout and stderr
	Write-Host "Getting ACR resource ID..." -ForegroundColor Gray
	$acrResourceId = az acr show --name $AcrName --query id -o tsv 2>$null
	
	if ($LASTEXITCODE -ne 0 -or [string]::IsNullOrWhiteSpace($acrResourceId)) {
		Write-Host "✗ Failed to retrieve ACR resource ID. Please verify the ACR name '$AcrName' is correct and you have access to it." -ForegroundColor Red
		exit 1
	}

	# Assign AcrPull role to the managed identity
	Write-Host "Assigning AcrPull role to managed identity..." -ForegroundColor Gray
	$roleAssignmentOutput = (az role assignment create `
		--assignee $principalId `
		--role "AcrPull" `
		--scope $acrResourceId) 2>&1

	if ($LASTEXITCODE -eq 0) {
		Write-Host "✓ Managed identity configured with AcrPull role`n" -ForegroundColor Green
	} else {
		Write-Host "✗ Failed to assign AcrPull role to Container App: $DevAppName" -ForegroundColor Red
		Write-Host "Managed Identity Principal ID: $principalId" -ForegroundColor Gray
		if ($roleAssignmentOutput) {
			Write-Host "Error details:" -ForegroundColor Gray
			$roleAssignmentOutput | ForEach-Object { Write-Host "  $_" -ForegroundColor Gray }
		}
		Write-Host "The Container App will not be able to pull images from ACR without this role assignment." -ForegroundColor Red
		Write-Host "Please manually assign the AcrPull role to the Container App's managed identity.`n" -ForegroundColor Yellow
		exit 1
	}
}

# Get the app URL
Write-Host "Getting application URL..." -ForegroundColor Yellow
$appUrl = az containerapp show `
	--name $DevAppName `
	--resource-group $ResourceGroup `
	--query properties.configuration.ingress.fqdn `
	--output tsv

Write-Host "`n========================================" -ForegroundColor Green
Write-Host "Development Environment Ready!" -ForegroundColor Green
Write-Host "========================================`n" -ForegroundColor Green

Write-Host "Development URL: " -NoNewline -ForegroundColor Cyan
Write-Host "https://$appUrl" -ForegroundColor Yellow

Write-Host "`nEndpoints:" -ForegroundColor Cyan
Write-Host "  Root:   https://$appUrl/" -ForegroundColor White
Write-Host "  Health: https://$appUrl/health" -ForegroundColor White
Write-Host "  API:    https://$appUrl/api/todo" -ForegroundColor White

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "Next Steps" -ForegroundColor Cyan
Write-Host "========================================`n" -ForegroundColor Cyan

Write-Host "1. Add secrets to development Container App:" -ForegroundColor Yellow
Write-Host "   - Go to Azure Portal" -ForegroundColor White
Write-Host "   - Navigate to Container Apps → $DevAppName" -ForegroundColor White
Write-Host "   - Secrets → Add the same secrets as production`n" -ForegroundColor White

Write-Host "2. Create a 'dev' branch for automatic deployments:" -ForegroundColor Yellow
Write-Host "   git checkout -b dev" -ForegroundColor White
Write-Host "   git push -u origin dev`n" -ForegroundColor White

Write-Host "3. Push to 'dev' branch to trigger deployment:" -ForegroundColor Yellow
Write-Host "   git push origin dev`n" -ForegroundColor White

Write-Host "✓ Setup complete!" -ForegroundColor Green
