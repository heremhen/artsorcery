# artsorcery

# ASP.NET 6+ MVC / TAILWIND WITH DAISYUI / CODE FIRST APPROACH

Install packages :
  - install-package Microsoft.EntityFrameworkCore.SqlServer
  - install-package Microsoft.EntityFrameworkCore.Tools

- Make sure to change the context class and the connection string in appsettings.json for your mssql db server.
- Create database using nuget package console: 
      "add-migration <migationName>", "update-database / update-database -verbose"
- Also run "NPM INSTALL" inside the solutions folder using powershell or terminal cmd where the tailwind.config.js is.
- Inspired by artstation somehow.

# Features:

- Login/Register
- User Profile
- Settings
- Upload Artwork
- Artwork Details
- Edit Artwork
- Delete Artwork
- Search
- Pagination
- Used GUID token to store user id for logout and other data fetching, i think.
