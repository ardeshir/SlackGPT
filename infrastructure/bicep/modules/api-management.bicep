@description('The virtual networks used by API Mangement.')
param networks array = []

@description('The name of the API Management service instance to create. This must be globally unique.')
param serviceName string

@description('The name of the API publisher. This information is used by API Management.')
param publisherName string

@description('The email address of the API publisher. This information is used by API Management.')
param publisherEmail string

@description('The name of the SKU to use when creating the API Management service instance. This must be a SKU that supports virtual network integration.')
@allowed([
  'Developer'
  'Premium'
])
param skuName string

@description('The number of worker instances of your API Management service that should be provisioned.')
param skuCount int

@description('The type of virtual network integration to deploy. In \'External\' mode, a public IP address will be associated with the API Management service instance. In \'Internal\' mode, the instance is only accessible using private networking.')
@allowed([
  'External'
  'Internal'
])
param virtualNetworkType string

var resTags = {
  displayName: 'APIM'
  DataClassification: 'Public'
  AppEnvironment: 'cloudops'
}

var tags = union(json(loadTextContent('../_tags.json')), resTags)

resource apiManagementService 'Microsoft.ApiManagement/service@2021-08-01' = {
  name: serviceName
  location: networks[0].location
  tags: tags
  sku: {
    name: skuName
    capacity: skuCount
  }
  properties: {
    publisherName: publisherName
    publisherEmail: publisherEmail
    virtualNetworkType: virtualNetworkType
    publicIpAddressId: networks[0].publicIpAddressId
    virtualNetworkConfiguration: {
      subnetResourceId: networks[0].apiManagementSubnetResourceId
    }
    additionalLocations: [for i in range(1, length(networks) - 1): {
      location: networks[i].location
      publicIpAddressId: networks[i].publicIpAddressId
      sku: {
        name: 'Premium'
        capacity: 1
      }
      virtualNetworkConfiguration: {
        subnetResourceId: networks[i].apiManagementSubnetResourceId
      }
    }]
  }
}

@description('API Management Backends.')
param backends array = []

resource apiBackends 'Microsoft.ApiManagement/service/backends@2022-04-01-preview' = [for backend in backends: {
  parent: apiManagementService
  name: backend.name
  properties: {
    protocol: 'http'
    url: backend.url
  }
}]

@description('API Management APIs.')
param apis array = []

resource apiSettings 'Microsoft.ApiManagement/service/apis@2022-04-01-preview' = [for api in apis: {
  parent: apiManagementService
  name: api.name
  properties:{
    displayName: api.displayName
    path: api.path
    serviceUrl: api.serviceUrl
    protocols: [
      'https'
    ]
    subscriptionRequired: api.subscriptionRequired
  }
}]

resource apiPolicy 'Microsoft.ApiManagement/service/apis/policies@2022-04-01-preview' = [for api in apis: {
  name: '${serviceName}/${api.name}/policy'
  dependsOn: [
    apiSettings
  ]
  properties: {
    format: 'xml'
    value: api.policy
  }
}]

output apiManagementInternalIPAddress string = apiManagementService.properties.publicIPAddresses[0]
output apiManagementProxyHostName string = apiManagementService.properties.hostnameConfigurations[0].hostName
output apiManagementDeveloperPortalHostName string = replace(apiManagementService.properties.developerPortalUrl, 'https://', '')
