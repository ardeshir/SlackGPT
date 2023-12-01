@description('API Management DNS records.')
param dns array = []

@description('API Mangement virtual networks.')
param networks array = []

// get tags
var resTags = {
  displayName: 'APIM'
  DataClassification: 'Public'
  AppEnvironment: 'cloudops'
}

var tags = union(json(loadTextContent('../_tags.json')), resTags)

resource privateDnsZone 'Microsoft.Network/privateDnsZones@2020-06-01' = {
  name: 'privatelink.azurewebsites.net'
  location: 'global'
  tags: tags
}

resource dnsRecord 'Microsoft.Network/privateDnsZones/A@2020-06-01' = [for record in dns: {
  name: record.name
  parent: privateDnsZone
  properties: {
    ttl: 10
    aRecords: [
      {
        ipv4Address: record.address
      }
    ]
  }
}]

resource virtualNetworkLink 'Microsoft.Network/privateDnsZones/virtualNetworkLinks@2020-06-01' = [for network in networks: {
  name: network.vnetName
  location: 'global'
  tags: tags
  parent: privateDnsZone
  properties: {
    registrationEnabled: false
    virtualNetwork: {
      id: resourceId('Microsoft.Network/virtualNetworks', network.vnetName)
    }
  }
}]
