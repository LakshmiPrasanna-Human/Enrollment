Login-AzureRmAccount

# Variables
$endpoint = 'mysfcluster123.southcentralus.cloudapp.azure.com:19000'
$thumbprint = '08CF1FE24695183F949327ADB2C6C2F31AD9196B'
$FindTypeClientthumbprint = '825595A92DAA608C437DCE86A809C95BE82C82EA'
$packagepath="D:\Architecture\azure\AzureServiceFabric\Enrollment\Enrollment\pkg\Release"

# Connect to the cluster using a client certificate.
Connect-ServiceFabricCluster -ConnectionEndpoint $endpoint `
          -KeepAliveIntervalInSec 10 `
          -X509Credential -ServerCertThumbprint $thumbprint `
          -FindType FindByThumbprint -FindValue $FindTypeClientthumbprint `
          -StoreLocation CurrentUser -StoreName My

# Remove an application instance
Remove-ServiceFabricApplication -ApplicationName fabric:/Application1

# Unregister the application type
Unregister-ServiceFabricApplicationType -ApplicationTypeName Application1 -ApplicationTypeVersion 1.0.0