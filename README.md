# Course Enrollment System

An ASP.NET Core MVC application for browsing online courses, viewing discounted tuition, and enrolling with student contact details.

## Features

- **Course catalog:** Browse courses with duration, original fees, discount percentages, and calculated final fees.
- **Course details:** Open a course using its `courseId` query string and submit an enrollment form.
- **My Course page:** Revisit the course selected during the current session at `/Course/MyCourse`.
- **Student details:** Save the enrolled student's name and email in browser cookies and display them on the confirmation and My Course pages.
- **Learner directory:** Retrieve name, email, and company data asynchronously from JSONPlaceholder.
- **State management:** Use a cookie for student details, session for the selected course, a query string for course details, and a hidden field for the enrollment course ID.
- **Caching:** Cache the course catalog in `IMemoryCache` for five minutes and enable response caching for course details.

## Requirements

- .NET 10 SDK
- Internet access for the JSONPlaceholder learner directory

## Run locally

From the project directory:

```powershell
dotnet restore
dotnet run
```

Open the local URL printed by `dotnet run` (the launch profile uses `http://localhost:5182`).

## Main routes

- `/Course` - course catalog and learner directory
- `/Course/Details?courseId=1` - details for a course
- `/Course/MyCourse` - the course selected in the current session

## Course fee calculation

`CalculateFinalFees()` applies the course's discount percentage to its listed fee.

## Technology

- ASP.NET Core MVC and Razor
- C# / .NET 10
- `IMemoryCache`, session state, cookies, and response caching
- JSONPlaceholder users API