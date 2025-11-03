# Security Configuration Guide

## Development Environment (User Secrets)

The following secrets are stored using .NET User Secrets for development:

### Setting up User Secrets

If you're setting up this project for the first time, run these commands:

```bash
cd CalapanCarRentalMVC

# Set Email Password
dotnet user-secrets set "EmailSettings:Password" "your-gmail-app-password"

# Set Google OAuth Client Secret
dotnet user-secrets set "Authentication:Google:ClientSecret" "your-google-client-secret"

# Optional: Set Database Password if needed
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "server=localhost;port=3306;database=calapan_car_rental;user=root;password=your-db-password"
```

### Viewing Current Secrets

```bash
dotnet user-secrets list
```

### Removing a Secret

```bash
dotnet user-secrets remove "EmailSettings:Password"
```

### Clearing All Secrets

```bash
dotnet user-secrets clear
```

## Production Environment

For production, use one of these secure methods:

### Option 1: Azure Key Vault (Recommended for Azure deployments)
- Store secrets in Azure Key Vault
- Configure your app to read from Key Vault using Managed Identity

### Option 2: Environment Variables
Set environment variables on your production server:
- `EmailSettings__Password`
- `Authentication__Google__ClientSecret`
- `ConnectionStrings__DefaultConnection`

Note: Use double underscores `__` to represent nested configuration in environment variables.

### Option 3: Azure App Service Configuration
In Azure Portal ? App Service ? Configuration ? Application Settings:
- Add each secret as an application setting
- These override appsettings.json values

## Security Best Practices

1. ? **Never commit secrets to Git**
2. ? **Use User Secrets for local development**
3. ? **Use Azure Key Vault or environment variables for production**
4. ? **Rotate secrets regularly**
5. ? **Use different secrets for different environments**
6. ? **Enable MFA on accounts with API access**
7. ? **Use Gmail App Passwords (not your actual Gmail password)**

## Secrets Currently Managed

- ? **EmailSettings:Password** - Gmail App Password
- ? **Authentication:Google:ClientSecret** - Google OAuth Secret
- ?? **ConnectionStrings:DefaultConnection** - Currently has empty password (consider securing if needed)

## Troubleshooting

If your app can't find secrets in development:
1. Verify User Secrets are set: `dotnet user-secrets list`
2. Check that UserSecretsId exists in your .csproj file
3. Ensure you're running in Development environment
