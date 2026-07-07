# What is it?
Equmap is a stock market performance visualization tool built with .NET 10 and SvelteKit. Even though there's a few 
similar apps, not many of them offer the ability to choose only the stocks you want to track, meaning with Equmap, you can create your custom set of stocks you want to visualize.
Another reason for me to create this app is the ability to choose which metrics you want to track, since some of the mentioned apps offer a limited number of such metrics.

The application attempts to follow Clean Architecture principles and is split into independent frontend and backend deployments.

* StockMapSvelte.Api
* StockMapSvelte.Application
* StockMapSvelte.Client
* StockMapSvelte.Domain
* StockMapSvelte.Infrastructure

The Client layer is using SvelteKit with Svelte5 and shadcn-svelte component library, other layers are using .NET 10.
# Folder structure

<pre>
root/
├── .github/
│   └── workflows/
│       ├── dotnet.yml
│       └── node.js.yml
├── src/
│   ├── StockMapSvelte.Api/
│   │   ├── Controllers/
│   │   ├── Middleware/
│   │   ├── Services/
│   │   ├── Requests/
│   │   ├── Properties/
│   │   ├── Program.cs
│   │   ├── DependencyInjection.cs
│   │   ├── StockMapSvelte.Api.csproj
│   │   ├── StockMapSvelte.Api.http
│   │   ├── appsettings.json
│   │   └── appsettings.Development.json
│   ├── StockMapSvelte.Application/
│   │   ├── Abstractions/
│   │   ├── DTOs/
│   │   ├── Enums/
│   │   ├── Exceptions/
│   │   ├── Facade/
│   │   ├── Services/
│   │   ├── UseCases/
│   │   ├── DependencyInjection.cs
│   │   └── StockMapSvelte.Application.csproj
│   ├── StockMapSvelte.Client/
│   │   ├── src/
│   │   │   ├── lib/
│   │   │   │   ├── Abstractions/
│   │   │   │   ├── assets/
│   │   │   │   ├── components/
│   │   │   │   ├── hooks/
│   │   │   │   ├── services/
│   │   │   │   ├── apiFetch.ts
│   │   │   │   ├── auth.svelte.ts
│   │   │   │   ├── portfolioFetch.ts
│   │   │   │   ├── utils.ts
│   │   │   │   └── index.ts
│   │   │   ├── routes/
│   │   │   │   ├── (index)/
│   │   │   │   ├── (protected)/
│   │   │   │   ├── confirm-email/
│   │   │   │   ├── forgot-password/
│   │   │   │   ├── link-account/
│   │   │   │   ├── login/
│   │   │   │   ├── privacy-policy/
│   │   │   │   ├── register/
│   │   │   │   ├── resend-email-confirmation/
│   │   │   │   ├── reset-password/
│   │   │   │   ├── +layout.server.ts
│   │   │   │   ├── +layout.svelte
│   │   │   │   └── layout.css
│   │   │   ├── app.d.ts
│   │   │   ├── app.html
│   │   │   └── hooks.server.ts
│   │   ├── static/
│   │   ├── .vscode/
│   │   ├── package.json
│   │   ├── package-lock.json
│   │   ├── vite.config.ts
│   │   ├── svelte.config.js
│   │   ├── tsconfig.json
│   │   ├── eslint.config.js
│   │   ├── components.json
│   │   ├── .prettierrc
│   │   ├── .prettierignore
│   │   ├── .npmrc
│   │   ├── .gitignore
│   │   └── README.md
│   ├── StockMapSvelte.Domain/
│   │   ├── Entities/
│   │   └── StockMapSvelte.Domain.csproj
│   └── StockMapSvelte.Infrastructure/
│       ├── BackgroundServices/
│       ├── Database/
│       ├── Extensions/
│       ├── Identity/
│       ├── Migrations/
│       ├── Repositories/
│       ├── Services/
│       ├── DependencyInjection.cs
│       └── StockMapSvelte.Infrastructure.csproj
├── tests/
│   └── StockMapSvelte.Tests/
│       ├── IntegrationTests/
│       ├── UnitTests/
│       └── StockMapSvelte.Tests.csproj
├── .gitignore
├── Dockerfile
├── StockMapSvelte.slnx
</pre>

# Environment variables required to run the app
Since the app is recommended to be hosted separately, meaning the frontend and backend are hosted on different domains/servers, here are the required env. variables for each of those two layers:

#### Frontend:
- PUBLIC_API_BASE_URL - the base URL of the API

#### Backend
- ConnectionStrings__DefaultConnection - connection string for the database
- Resend__ApiToken - token for the resend email service used to send confirmation and password reset emails
- Resend__FromEmail - the email address used as the sender for the emails sent through the resend service
- FrontendUrl - the URL of the frontend app, used in the emails sent to users to link back to the frontend
- Authentication__Google__ClientSecret - client secret for Google OAuth authentication
- Authentication__Google__ClientId - client id for Google OAuth authentication
- Seeding__AdminPasswordHash - password hash for the default admin user created when seeding the database
- Seeding__ManagerPasswordHash - password hash for the default manager user created when seeding the database
- AllowedOrigins__0 - allowed origin for CORS policy, should be set to the URL of the frontend app
- AllowedOrigins__1 - allowed origin for CORS policy, should be set to the www. version of the frontend URL

#### Configuration of the environment variables for use in local development:

AllowedOrigins can be configured for development in the appsettings.Development.json file:

```
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedOrigins": [
    "http://localhost:5173",
    "https://localhost:5173"
  ]
}
```

The PUBLIC_API_BASE_URL variable can be configured for development in the .env file in the root of the StockMapSvelte.Client project:

```
PUBLIC_API_BASE_URL=https://localhost:7086

#https://localhost:7086

#http://localhost:5289
```

rest of the environment variables should be stored in .NET user secrets configured in the StockMapSvelte.Api layer:

```
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=stock-map-svelte;Username=postgres;Password=rootroot",
  },
  "Resend": {
    "ApiToken": "xxxxxx",
    "FromEmail": "xxxxxx"
  },
  "FrontendUrl": "https://localhost:5173",
  "Authentication": {
    "Google": {
      "ClientSecret" : "xxxxxx",
      "ClientId": "xxxxxx"
    }
  },
  "Seeding": {
    "AdminPasswordHash": "xxxxxx",
    "ManagerPasswordHash": "xxxxxx"
  }
}
```


# API layer

## Endpoints

### StockMapSvelte.Api - auto generated identity endpoints and other mapped endpoints

```
get /treemap-data/{portfolioId}/stream
```
- SSE stream endpoint what streams treemap data for a certain portfolio on stock data change
```
post /register
```
- Registers a new user, an email sender automatically sends a code to the email - /confirmEmail is used to confirm this email
```
post /login
```
- Authenticates a user with email and password. Returns an access token and refresh token
```
post /refresh
```
- Returns a new access and refresh token when a refresh token is provided
```
get /confirmEmail
```
- Sets the users email as verified in the database based on the passed token that has been sent to the user's email address (setting the email as verified is required)
```
post /resendConfirmationEmail
```
- Send the confirmation token to the desired email address (if the user didn't receive the token on the first try)
```
post /forgotPassword
```
- Sends a password reset link to the provided email address
```
post /resetPassword
```
- Resets the password for a provided email address if a valid reset token is passed
```
post /manage/2fa
```
- Enables, disables, or configures two-factor authentication for the authenticated user
```
get /manage/info
```
- Get the email of the user and whether the email is confirmed 
```
post /manage/info
```
- Updates the current authenticated user's account info
### Account
```
get /account/info
```
- Returns info about the current authenticated user's account - HasPasswordConfigured, HasExternalLoginConfigured, ExternalLogins, IsAdmin, IsManager, IsCustomer, Email, IsEmailConfirmed
```
delete /account/me
```
- Deletes the whole account for the authenticated user
```
post /account/set-password
```
- Used for adding a password to passwordless account
```
delete /account/remove-google-external-login
```
- Removes google external login if the account has password set

### OAuth
```
get /oauth/google-login
```
- Initiates the Google OAuth2 authentication flow by redirecting to Google's consent screen
```
get /oauth/google-response
```
- Callback endpoint that handles the response from Google after the user authenticates
```
post /oauth/link-oauth-confirm
```
- Links a Google OAuth account to an existing local user account

### Portfolio
```
get /portfolios
```
- ...
```
post /portfolios
```
- ...
```
get /portfolios/me
```
- Get all portfolios for the current user
```
delete /portfolios/{portfolioId}
```
- ...
```
put /portfolios/{portfolioId}
```
- ... 
```
get /portfolios/{portfolioId}
```
- ...

### Role
```
[DEPRECATED]
get /roles/is-admin
get /roles/is-manager
get /roles/is-customer
```
### Stock
```
get /stocks/{stockId}
```
- ...
```
put /stocks/{stockId}
```
- ...
```
delete /stocks/{stockId}
```
- ...
```
get /stocks/possible-to-add
```
- Get stocks that are possible to be added to a given portfolio based on searching them against a filter
```
get /stocks
```
- ...
```
post /stocks
```
- ...

### StockProfiles
```
get /stock-profiles
```
- ...
### TreemapData
```
get /treemap-data/{portfolioId}
```
- DEPRECATED - use the stream version
### UserAction
```
post /logout
```
- ...
### User
```
get /users
```
- ...
```
get /users/{userId}
```
- ...
```
delete /users/{userId}
```
- ...
### UserSettings
```
get /user-settings/default-portfolio
```
- ...
```
put /user-settings/default-portfolio
```
- ...
The following four enpoints are deprecated:
```
get /user-settings/toast-delay
put /user-settings/toast-delay
get /user-settings/toast-auto-hide
put /user-settings/toast-auto-hide
```

More detailed information about the API endpoints can be found at https://localhost:7086/scalar/v1 when running the app in development mode.

---
- Controllers providing endpoints for the client to interact with the application
- Middleware for intercepting thrown exceptions across the app and returning appropriate responses to the client
- Properties containing the app's configuration settings - launchSettings.json lets you configure the app's launch settings, such as the URL and port it runs on
- Requests containing the request models for the API endpoints - this could be moved into a dedicated layer in the future containing also the DTO's declared in Application layer
- Services containing logic related to the API layer, currently only UserContext making use of the HttpContext through IHttpContextAccessor
- DependencyInjection.cs featuring AddPresentation method called in Program.cs to register API related services - CORS policy, cookie configuration, rate limiting

# Application layer
- Abstractions containing interfaces for the application layer
- DTO's containing the data transfer objects
- Enums
- Exceptions containing custom exceptions inheriting from AppException class
The DTO's, enums, exceptions and requests from the Api layer could be moved into a dedicated common layer
- Facades wrapping the use cases since the app doesn't utilize the Mediator pattern (yet)
- UseCases containing the use cases for the app, which are called from the API layer through the facades
- DependencyInjection.cs featuring AddApplication method called in Program.cs to register application related services
---
# Client layer
### src/
#### lib/
- Abstractions containing types shared across the app
- assets containing the app's assets, such as icons and images
- components containing the app's custom or imported components
- services containing only logic related to rendering the treemap data at this moment
- apiFetch.ts helper wrapper file to make API calls easier to manage
- auth.svelte.ts containing AuthManager class featuring methods related to user authentication and authorization, such as login, logout, register
- dedicated portfolioFetch.ts file to fetch treemap data for the current portfolio
---
#### routes/
- various routes for the app, such as login, register, portfolio, etc. Each route contains a +layout.svelte file which is the main component for that route and a +layout.server.ts file which is used to load data for that route on the server side before rendering the page & to run auth hooks to prevent leaking the html structure
---
- hooks.server.ts containing hooks to run on the server side before rendering the page, such as checking if the user is authenticated and authorized to access the page, and redirecting them if not

# Domain layer
- Entities containing the app's entities, such as User, Portfolio, Stock, etc.
These entities are supposed to be extended with methods to prevent anemic domain models

# Infrastructure layer
This layer contains implementation of various external services used in the app such as the database, client for fetching stock data, email sender

### User roles

Admin - explicit role
Manager - explicit role
Customer - explicit role (DEPRECATED - IsCustomer policy is used instead for now - every authenticated user is a customer) 

---
- BackgroundServices - StockDataUpdateTimedService implementing the BackgroundService class used to orchestrate running periodic fetching of the stock data from an external provider
It makes use of StockUpdateJitter which tells the service whether to run the current round of fetching and the delay until next attempt. If an update is made, the service publishes a message alerting all subscribers to TreeMapUpdateNotifier (treemap data stream endpoint)
- Database contains seeding logic and ApplicationDbContext.cs where seeding and mapping of relationships between entities is done. The project is set up to use the PostgreSQL database
- Extentions folder contains logic for pagination, sorting, search designed to be translated into EntityFramework/database queries
- Identity defines the roles, application user entity, role entity and service that uses Identity API (SignInManager, UserManager)
- Migrations folder containing generated database migrations
- Repositories containing classes which work directly with EntityFramework to get data from and to the database as well as getting data from external providers (this could be better to move into a dedicated folder), the folder uses the decorator pattern that extends those classes through their cached versions, the app makes use of the HybridCache
- Services such as the implementation of the IEmailSender
- DependencyInjection.cs configuring the database, Identity, ResendClient, GoogleOpenId, HybridCache, Authorization policies - every authenticated user in the app is a Customer - this is subject to change in the future

# Tests folder
This folder contains integration and unit tests for the backend (and possibly frontend in the future)
- The integration tests folder offers an IntegrationTestWebApplicationFactory that makes use of Testcontainers (PostgreSqlContainer) as well as a TestAuthHandler that makes it possible to configure authentication/authorization for clients created by the factory.
- The unit tests make use of the Moq library to mock the apps repositories etc. and configure their behavior

# TODO Features:
- Introduce limit on the number of portfolios the user can have at one moment & limit on the number of stocks in one portfolio - this can be then upraded based on the user's role (tiers)