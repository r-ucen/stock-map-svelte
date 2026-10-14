# What is it?
Equmap is a stock market performance visualization tool built with .NET and SvelteKit. Even though there's a few 
similar apps, not many of them offer the ability to choose only the stocks you want to track, meaning with Equmap, you can create your custom set of stocks you want to visualize.
Another reason for me to create this app is the ability to choose which metrics you'd like to track, since some of the apps offer a limited number of such metrics.

The application is split into multiple layers:

* Api - base project; .NET 10 Web API, controllers, middleware...
* Application - business logic, use cases...
* Client - SvelteKit frontend using svelte5 and svelte-shadcn compnent library
* Domain - entities...
* Infrastructure - database, external services...

---

## Configuration of the environment variables for use in local development

For local development, the only secrets you need to set are the following:

PUBLIC_API_BASE_URL -  configured in a .env file in the root of the StockMapSvelte.Client project:

```
PUBLIC_API_BASE_URL=https://localhost:7086
```

Rest of the environment variables should be stored in .NET user secrets configured in the StockMapSvelte.Api layer:

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

---

# Environment variables required to host the app

#### Frontend
- PUBLIC_API_BASE_URL - the base URL of the API, for example https://api.yourdomain.com

#### Backend
- Resend__ApiToken - token for the resend email service used to send confirmation and password reset emails
- Resend__FromEmail - the email address used as the sender for the emails sent through the resend service
- FrontendUrl - the URL of the frontend app, used in the emails sent to users to link back to the frontend
- Authentication__Google__ClientSecret - client secret for Google OAuth authentication
- Authentication__Google__ClientId - client id for Google OAuth authentication
- Seeding__AdminPasswordHash - password hash for the default admin user created when seeding the database
- Seeding__ManagerPasswordHash - password hash for the default manager user created when seeding the database
- AllowedOrigins__0 - allowed origin for CORS policy, should be set to the URL of the frontend app
- AllowedOrigins__1 - allowed origin for CORS policy, should be set to the www. version of the frontend URL
- MAINTENANCE_PAGE_URL - the URL of a JSON file containing the "status": 503 and "message": "The app is under maintenance." fields
- ERROR_PAGE_URL - the URL of a JSON file containing the "status": 500 and "message": "An unexpected error occurred." fields
- DataProtectionCert - base64 encoded certificate used for encrypting the master data protection keys which are used to encrypt/decrypt cookies 
- DataProtectionPassword - password used to decrypt the DataProtectionCert certificate
- DATABASE_URL - the url of the database (might be a connection string depending on the provider)
- CookieDomain - the domain for which the cookies are valid (can be set to .yourdomain.com)


### Heroku deployment (only API + db)
1) Create a new app on Heroku, name it stock-map-svelte
2) Add a PostgreSQL database to the app
3) Add the required environment variables to the app's settings on Heroku
4) Also, if you have a custom domain, add the domain to the app's settings on heroku and register the DNS target in the domain's provider
5) Seed the database or migrate it from somewhere else: seed by getting the DATABASE_URL from the app's settings -> config vars and convert it to a connection string and exchange it temporarily in the .NET user secrets for the local db connection string, the update the database using the most recent migration (in Rider IDE right click the Infrastructure project -> Entity Framework Core -> Update Database); to migrate run pg_dump against the existing populated database and then pg_restore with flags --no-owner --no-privileges on the generated .dump file using the DATABASE_URL connection string from the app's settings

Migration:
```
pg_dump -F c -v -d "DATABASE_URL or CONNECTION STRING" -f backup.dump
```
```
pg_restore --no-owner --no-privileges --clean -d "DATABASE_URL or CONNECTION STRING" -v backup.dump
```

7) Install the Heroku CLI
8) run "heroku login" (if path not configured on Windows, run export PATH="$PATH:/c/Program Files/heroku/bin" after installing the CLI via the installer)
```
heroku login
```
9) run the docker engine locally
10) run
```
heroku container:login
```
11) run
```
heroku git:remote -a stock-map-svelte
```
12) build the Dockerfile in the current directory and push the Docker image
```
heroku container:push web
```
13) deploy the changes
```
heroku container:release web
```
14) enable automated SSL certificate management by running
```
heroku certs:auto:enable
```

---

# API layer

## Endpoints

### StockMapSvelte.Api
Auto generated identity endpoints and other mapped endpoints

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

Further information about all of the API endpoints can be found at https://localhost:7086/scalar/v1 when running the app in development mode.

The API layer then contains also the following:
- Middleware for intercepting thrown exceptions across the app and returning appropriate status codes and messages
- Properties containing the app's configuration settings - launchSettings.json lets you configure the app's launch settings, such as the URL and port it runs on
- Requests containing the request models for the API endpoints
- DependencyInjection.cs featuring AddPresentation method called in Program.cs to register API related services - CORS policy, cookie configuration, rate limiting

---

# Application layer

### Abstractions 
Containing interfaces for the application layer
### DTOs
The data transfer objects
### Enums
### Exceptions
Containing custom exceptions inheriting from AppException class
### Extensions
Containing methods that extend the domain entities with convert to DTO methods
### Facades
Wrapping the use cases since the app doesn't utilize the Mediator pattern (yet)
### UseCases
Containing the use cases for the app, which are called from the API layer through the facades
### DependencyInjection.cs 
Featuring AddApplication method called in Program.cs to register application related services

---

# Client layer

# src/lib/
### Abstractions
Containing types shared across the app
### assets
App's assets, such as icons and images
### components
App's custom or imported components
### services 
Containing only logic related to rendering the treemap data at this moment

### apiFetch.ts 
Helper wrapper file to make API calls easier to manage
### auth.svelte.ts 
Containing AuthManager class featuring methods related to user authentication and authorization, such as login, logout, register
### portfolioFetch.ts 
Dedicated file to fetch treemap data for the current portfolio

## src/routes/
Various routes for the app, such as login, register, portfolio, etc. Each route contains a +layout.svelte file which is the main component for that route and a +layout.server.ts file which is used to load data for that route on the server side before rendering the page & to run auth hooks to prevent leaking the html structure

## src/
Hooks.server.ts containing hooks to run on the server side before rendering the page, such as checking if the user is authenticated and authorized to access the page, and redirecting them if not

---

# Domain layer
### Entities
Containing the app's entities, such as User, Portfolio, Stock, etc.
### Exceptions
Domain specific exceptions

---

# Infrastructure layer
This layer contains implementation of various external services used in the app such as the database, client for fetching stock data, email sender

### StockDataUpdateTimedService
A background service that runs periodically, 
first when the app starts and then every x minutes, 
where x is given by the StockUpdateJitter service. 
The StockUpdateJitter also decides whether to run the background service 
and when not to run it (for example weekends). 
The service fetches new stock information using 
the implemented IStockClient and saves the new data to the 
database (wrapped in the IStockUpdateService's UpdateAsync method). 
Then it publishes a message to the TreeMapUpdateNotifier service 
which notifies all subscribers to the treemap data stream endpoint 
that new data is available.

### Cache
Contains the implementation of the ICacheService called HybridCacheService which is just a wrapped around the .NET's HybridCache. Also contains classes for constructing cache keys and tags by calling their methods.

### Database
Contains seeding logic and ApplicationDbContext.cs where seeding and mapping of relationships between entities is done. The project is set up to use the PostgreSQL database

### Extensions
Various extensions either for pagination, search, filtering on database queries or for simplifying the calling of non-modifiable methods of the .NET framework.

### Identity
Defines the roles, ApplicationUser entity, Role entity and service that uses Identity API (SignInManager, UserManager), also provides classes for interacting with user authentication and authorization

#### User roles

- Admin - explicit role
- Manager - explicit role
- Customer - explicit role (DEPRECATED - IsCustomer policy is used instead for now - every authenticated user is a customer)

### Migrations 
Folder containing generated database migrations

### Repositories
Containing classes for interacting with the database through EF.

### Services
Implementations of various services used in the app, such as IEmailSender, IStockClient...

### DependencyInjection.cs
Configuring the database, Identity, ResendClient, GoogleOpenId, HybridCache, Authorization policies - every authenticated user in the app is a Customer

---

# Tests layer
This folder contains integration and unit tests for the backend (and possibly frontend in the future)
- The integration tests folder offers an IntegrationTestWebApplicationFactory that makes use of Testcontainers (PostgreSqlContainer) as well as a TestAuthHandler that makes it possible to configure authentication/authorization for clients created by the factory. The integration tests cover some of the apps use cases.
- The unit tests cover just the StockUpdateJitter methods so far.