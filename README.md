## How to scaffold database (EF Core DB First approach)
1. Install the dotnet-ef tool globally
   ```
   dotnet tool install --global dotnet-ef --version 8.0.5
   ```
2. Execute the command
   ```
   dotnet ef dbcontext scaffold "ConnectionString" Microsoft.EntityFrameworkCore.SqlServer -o OutputFolder
   ```
