param location string = 'East US' 
param envName string   
param appServicePlan string  
param functionApp string  
param storageAccount string  
param vnetName string  
param subnetName string  
param keyVaultName string  
param keyVaultSecretName string  
param appIdObjectId string  

var resTags = {
  displayName: 'AzureOpenAI'
  DataClassification: 'Restricted'
  AppEnvironment: 'cloudops'
}

var tags = union(json(loadTextContent('_tags.json')), resTags)

var storageAccountName = '${storageAccount}${envName}'
var appServicePlanName = '${appServicePlan}-${envName}'
var functionAppName = '${functionApp}-${envName}'

// APIM & Networking 

@description('The virtual network location, IP address prefix (CIDR range), subnet IP address prefix (CIDR range) and region suffix.')
param locations array = [
  {
    name: resourceGroup().location
    ipPrefix: '10.0.0.0/16'
    subnetIPPrefix: '10.0.0.0/24'
    regionSuffix: ''
  }
]

@description('The name of the API Management service instance to create. This must be globally unique.')
param apiManagementServiceName string = 'apim-fsdi-unity-premium'

@description('The name of the API publisher. This information is used by API Management.')
param apiManagementPublisherName string = 'CDS'

@description('The email address of the API publisher. This information is used by API Management.')
param apiManagementPublisherEmail string = 'fs_admin@cargill.com'

@description('The name of the SKU to use when creating the API Management service instance. This must be a SKU that supports virtual network integration.')
@allowed([
  'Developer'
  'Premium'
])
param apiManagementSku string = 'Premium'

@description('The number of worker instances of your API Management service that should be provisioned.')
param apiManagementSkuCount int = 1

@description('API Management Backends.')
param backends array = []

@description('API Management APIs.')
param apis array = []

@description('API Management DNS records.')
param dns array = []

var vnetPrefix = 'VNet-cops'
var nsgPrefix = 'NSGCOPS'
var apimSubnet = 'apim' 

// get tags
var resTags = {
  displayName: 'APIM'
  DataClassification: 'Public'
  AppEnvironment: 'cloudops'
}

var tags = union(json(loadTextContent('_tags.json')), resTags)

resource publicIPAddresse_we 'Microsoft.Network/publicIPAddresses@2023-05-01' = {
  name: 'apim-we'
  location: 'westeurope'
  tags: tags
  sku: {
    name: 'Standard'
    tier: 'Regional'
  }
  zones: [
    '3'
    '2'
    '1'
  ]
  properties: {
    ipAddress: '4.245.32.170'
    publicIPAddressVersion: 'IPv4'
    publicIPAllocationMethod: 'Static'
    idleTimeoutInMinutes: 4
    dnsSettings: {
      domainNameLabel: 'apimcds'
      fqdn: 'apimcds.westeurope.cloudapp.azure.com'
    }
    ipTags: []
    ddosSettings: {
      protectionMode: 'VirtualNetworkInherited'
    }
  }
}

resource publicIPAddresse_us 'Microsoft.Network/publicIPAddresses@2023-05-01' = {
  name: 'apim'
  location: 'eastus'
  tags: tags
  sku: {
    name: 'Standard'
    tier: 'Regional'
  }
  zones: [
    '3'
    '2'
    '1'
  ]
  properties: {
    ipAddress: '20.242.138.166'
    publicIPAddressVersion: 'IPv4'
    publicIPAllocationMethod: 'Static'
    idleTimeoutInMinutes: 4
    dnsSettings: {
      domainNameLabel: 'apimcds'
      fqdn: 'apimcds.eastus.cloudapp.azure.com'
    }
    ipTags: []
    ddosSettings: {
      protectionMode: 'VirtualNetworkInherited'
    }
  }
}

// Manual virtual network peering must be in place between the API management vnets and all backend service vnets.
module network 'modules/network.bicep' = [for loc in locations: {
  name: 'network${loc.regionSuffix}'
  params: {
    location: loc.name
    vnetIPPrefix: loc.ipPrefix
    apiManagementSubnetIPPrefix: loc.subnetIPPrefix
    regionSuffix: loc.regionSuffix
    vnetNamePrefix: vnetPrefix
    nsgNamePrefix: nsgPrefix
    apiManagementSubnetName: apimSubnet 
  }
}]

var networks = [for location in locations: {
  vnetName: '${vnetPrefix}${location.regionSuffix}'
  location: location.name
  apiManagementSubnetResourceId: resourceId('Microsoft.Network/virtualNetworks/subnets', '${vnetPrefix}${location.regionSuffix}', apimSubnet) 
  publicIpAddressId: resourceId('Microsoft.Network/publicIPAddresses', concat('apim', '${location.regionSuffix}'))
}]

module privateDnsZone 'modules/api-management-dns.bicep' = {
  name: 'privatelink.azurewebsites.net'
  params: {
    dns: dns
    networks: networks
  }
}

var apiManagementVirtualNetworkType = 'External' 
module apiManagement 'modules/api-management.bicep' = {
  name: 'api-management'
  params: {
    networks: networks
    serviceName: apiManagementServiceName
    publisherName: apiManagementPublisherName
    publisherEmail: apiManagementPublisherEmail
    skuName: apiManagementSku
    skuCount: apiManagementSkuCount
    virtualNetworkType: apiManagementVirtualNetworkType
    backends: backends
    apis: apis
  }
}

resource apiManagementService 'Microsoft.ApiManagement/service@2021-08-01' existing = {
  name: apiManagementServiceName
  
  resource globalPolicy 'policies' = {
    name: 'policy'
    properties: {
      value: loadTextContent('api-management-policies/global.xml')
      format: 'xml'
    }
  }
}


// Function & WEB

resource storageAccountResource 'Microsoft.Storage/storageAccounts@2022-09-01' = {
  name: storageAccountName
  location: 'eastus'
  tags: tags
  sku: {
    name: 'Standard_LRS'
    tier: 'Standard'
  }
  kind: 'Storage'
  properties: {
    minimumTlsVersion: 'TLS1_0'
    allowBlobPublicAccess: true
    networkAcls: {
      bypass: 'AzureServices'
      virtualNetworkRules: []
      ipRules: []
      defaultAction: 'Allow'
    }
    supportsHttpsTrafficOnly: true
    encryption: {
      services: {
        file: {
          keyType: 'Account'
          enabled: true
        }
        blob: {
          keyType: 'Account'
          enabled: true
        }
      }
      keySource: 'Microsoft.Storage'
    }
  }
} 
  
resource appServicePlanResource 'Microsoft.Web/serverfarms@2022-03-01' = {  
  name: appServicePlanName  
  location: location  
  tags: tags
  kind: 'linux'
  sku: {
    name: 'P1v3'
    tier: 'PremiumV3'
    size: 'P1v3'
    family: 'Pv3'
    capacity: 1
  }
  properties: {
    perSiteScaling: false
    elasticScaleEnabled: false
    maximumElasticWorkerCount: 1
    isSpot: false
    reserved: true
    isXenon: false
    hyperV: false
    targetWorkerCount: 0
    targetWorkerSizeId: 0
    zoneRedundant: false
    virtualNetworkProfile: {  
      id: resourceId('Microsoft.Network/virtualNetworks/subnets', vnetName, subnetName)  
    }  
    keyVaultReferenceIdentity: {  
      userAssignedIdentities: [  
        appIdObjectId  
      ]  
    } 
  } 
}  
  
resource functionAppResource 'Microsoft.Web/sites@2022-03-01' = {  
  name: functionAppName  
  location: location  
  tags: tags
  kind: 'functionapp,linux'  
  identity: {
    type: 'SystemAssigned'
  }
  properties: {  
    serverFarmId: appServicePlanResource.id  
    httpsOnly: true 
    vnetRouteAllEnabled: true
    siteConfig: {  
      appSettings: [   
        {
          name: 'AZURE_CLIENT_ID'
          value: ''
        }
        {
          name: 'Azure_Subscription_ID'
          value: subscription().subscriptionId
        }
        {  
          name: 'AzureWebJobsStorage'  
          value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccountName};AccountKey=${listKeys(storageAccountResource.id, storageAccountResource.apiVersion).keys[0].value};EndpointSuffix=core.windows.net'  
        }  
        {  
          name: 'FUNCTIONS_EXTENSION_VERSION'  
          value: '~4'  
        }  
        {
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: 'dotnet'
        }
        {  
          name: 'OpenAI_API_KEY'  
          value: '@Microsoft.KeyVault(SecretUri=https://${keyVaultName}.vault.azure.net/secrets/${keyVaultSecretName}/)'  
        }  
      ]  
      linuxFxVersion: 'DOTNETCORE|6.0'
      ftpsState: 'Disabled'      
      alwaysOn: true
    }  
  }  
  dependsOn: [  
    storageAccountResource   
    appServicePlanResource  
  ]  
}  
  
output endpoint string = functionAppResource.properties.defaultHostName  

