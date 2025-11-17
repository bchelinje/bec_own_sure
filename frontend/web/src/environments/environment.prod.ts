export const environment = {
  production: true,
  apiUrl: 'https://api.deviceownership.com/api/v1',
  oauth: {
    clientId: 'web-client',
    authorizationUrl: 'https://api.deviceownership.com/connect/authorize',
    tokenUrl: 'https://api.deviceownership.com/connect/token',
    logoutUrl: 'https://api.deviceownership.com/connect/logout',
    scopes: ['openid', 'profile', 'email', 'device', 'marketplace']
  }
};
