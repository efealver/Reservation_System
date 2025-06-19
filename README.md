This is a web-based classroom reservation system developed using ASP.NET Core Razor Pages and Entity Framework Core. The system is designed to manage classroom reservations in a university setting, with role-based access for Admins and Instructors.

 Project Objectives
- Provide a secure login system for users with role-based access.
- Enable instructors to request recurring classroom reservations within academic terms.
- Allow admins to approve or reject reservations.
- Prevent reservations on official holidays and weekends.
- Send email notifications for feedback submissions.
- Visualize reservations and holidays on a full-featured calendar view.

### Admin
- Manage classrooms, terms, users, and logs.
- Approve or reject reservation requests.
- View all reservations and feedback.
- Access system logs.

### Instructor
- Create and manage their own reservations.
- View only their reservations.
- Submit feedback.
- Access their personal calendar.

## Technologies Used

- ASP.NET Core Razor Pages
- Entity Framework Core
- MS SQL Server
- Bootstrap 5
- FullCalendar.js
- Google Calendar API (for holiday detection)
- SMTP / Email support (for feedback notifications)

##
- Clone the repo
- dotnet restore
- dotnet ef database update
- dotnet run
- fill appsettings.json and credentials.json with your email service and api data
