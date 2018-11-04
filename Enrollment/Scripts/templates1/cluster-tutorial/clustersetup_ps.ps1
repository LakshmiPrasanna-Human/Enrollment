
param(
 [Parameter(Mandatory=$True)]
 [string]
 $subscriptionId,

 [Parameter(Mandatory=$True)]
 [string]
 $resourceGroupName,

 [Parameter(Mandatory=$True)]
 [string]
 $templateFilePath = "template.json"


)
Install-Module AzureRM -Scope CurrentUser
$ErrorActionPreference = "Stop"
Write-Host "Starting in...";


# Variables.
$UserName = "Manjunath_s07@infosys.com"
$SecurePassword = "Infy$123"
#$groupname = "TestGroupCluster"
$clusterloc="southcentralus"  # must match the location parameter in the template
#$templatepath="D:\Architecture\azure\AzureServiceFabric\Scripts\service-fabric-scripts-and-templates-master\templates\cluster-tutorial"

$certpwd="q6D7nN%6ck@6" | ConvertTo-SecureString -AsPlainText -Force
$certfolder="d:\mycertificates\"
$clustername = "mysfclusterSample"
$vaultname = "TestKeyVaultCluster"
$vaultgroupname="MyKeyVault"
$subname="$clustername.$clusterloc.cloudapp.azure.com"


$Error.Clear()
Set-PSRepository -Name PSGallery -InstallationPolicy Trusted

# sign in to your Azure account and select your subscription

#    $SecurePassword = ConvertTo-SecureString -String $password -AsPlainText -Force
#   $credential = New-Object -TypeName System.Management.Automation.PSCredential -ArgumentList ($UserName, $SecurePassword)
#   Login-AzureRmAccount -Credential $credential
Login-AzureRmAccount
 
#Set-AzureRmContext -SubscriptionId 02a0c632-86b4-4972-8b2a-c20752d297f8
Set-AzureRmContext -SubscriptionId $subscriptionId

# Create a new resource group for your deployment and give it a name and a location.
$resourceGroup = Get-AzureRmResourceGroup -Name $resourceGroupName -ErrorAction SilentlyContinue
if(!$resourceGroup)
{
    Write-Host "Creating resource group '$resourceGroupName' in location $clusterloc";
    New-AzureRmResourceGroup -Name $resourceGroupName -Location $clusterloc -Verbose 
}
else{
    Write-Host "Using existing resource group '$resourceGroupName'";
}

#New-AzureRmResourceGroup -Name $resourceGroupName -Location $clusterloc

Write-Host "Testing deployment...";
$testResult = Test-AzureRmResourceGroupDeployment -ResourceGroupName $resourceGroupName -TemplateFile "$templateFilePath\vnet-cluster.json" -TemplateParameterFile $templateFilePath\vnet-cluster.parameters.json -ErrorAction Stop;
if ($testResult.Count -gt 0)
{
	write-host ($testResult | ConvertTo-Json -Depth 5 | Out-String);
	write-output "Errors in template - Aborting";
	exit;
}

Write-Host "Starting Service Fabric Cluster...";


# Create the Service Fabric cluster.
New-AzureRmServiceFabricCluster  -ResourceGroupName $resourceGroupName -TemplateFile "$templateFilePath\vnet-cluster.json" `
-ParameterFile "$templateFilePath\vnet-cluster.parameters.json"  `
 -CertificatePassword $certpwd `
-CertificateOutputFolder $certfolder -KeyVaultName $vaultname -KeyVaultResouceGroupName $vaultgroupname -CertificateSubjectName $subname


