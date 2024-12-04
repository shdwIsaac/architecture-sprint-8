const keycloakConfig = {
    url: 'https://localhost:8080/auth',
    realm: 'reports-realm',
    clientId: 'reports-frontend',
    pkceMethod: 'S256', // Включаем PKCE
};

export default keycloakConfig