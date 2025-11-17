export const environment = {
  production: false,
  apiUrl: 'http://localhost:5000/api/v1',
  oauth: {
    clientId: 'web-client',
    authorizationUrl: 'http://localhost:5000/connect/authorize',
    tokenUrl: 'http://localhost:5000/connect/token',
    logoutUrl: 'http://localhost:5000/connect/logout',
    scopes: ['openid', 'profile', 'email', 'device', 'marketplace']
  }
};
