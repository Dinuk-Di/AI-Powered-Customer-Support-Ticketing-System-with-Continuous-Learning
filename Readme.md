# Step into your workspace folder
mkdir CustomerSupportTicketingSystem
cd CustomerSupportTicketingSystem

# Create solution file
dotnet new sln -n CustomerSupportTicketingSystem

# Create src and tests directories
mkdir src tests docker kubernetes docs build

# Create microservice project (e.g., TicketManagementService)
dotnet new webapi -n NotificationService -o ./src/NotificationService

# Create corresponding test project
dotnet new xunit -n NotificationService.Tests -o ./tests/NotificationService.Tests

# Add projects to solution
dotnet sln add ./src/NotificationService/NotificationService.csproj
dotnet sln add ./tests/NotificationService.Tests/NotificationService.Tests.csproj
