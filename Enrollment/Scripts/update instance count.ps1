Connect-ServiceFabricCluster 
Update-ServiceFabricService -ServiceName fabric:/Enrollment/EnrollmentAPI -Stateless -InstanceCount 3 -Force 


# Variables
$endpoint = 'mysftestcluster.southcentralus.cloudapp.azure.com:19000'
$thumbprint = '2779F0BB9A969FB88E04915FFE7955D0389DA7AF'
$packagepath="C:\Users\sfuser\Documents\Visual Studio 2017\Projects\MyApplication\MyApplication\pkg\Release"

# Connect to the cluster using a client certificate.
# Connect-ServiceFabricCluster -ConnectionEndpoint $endpoint `
#          -KeepAliveIntervalInSec 10 `
#          -X509Credential -ServerCertThumbprint $thumbprint `
#          -FindType FindByThumbprint -FindValue $thumbprint `
#          -StoreLocation CurrentUser -StoreName My
Connect-ServiceFabricCluster 

# Remove an application instance
Remove-ServiceFabricApplication -ApplicationName fabric:/Enrollment

# Unregister the application type
Unregister-ServiceFabricApplicationType -ApplicationTypeName EnrollmentType -ApplicationTypeVersion 1.0.0


Connect-ServiceFabricCluster 
Update-ServiceFabricService -ServiceName fabric:/Enrollment/EnrollmentStateFullService -Stateless -Confirm -Force -InstanceCount 5 