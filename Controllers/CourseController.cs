using System.Net.Http.Json;
using CourseEnrollmentSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace CourseEnrollmentSystem.Controllers;

public class CourseController : Controller
{
    private const string CoursesCacheKey = "course-list";
    private const string StudentNameCookie = "StudentName";
    private const string StudentEmailCookie = "StudentEmail";
    private const string SelectedCourseSessionKey = "SelectedCourseId";
    private readonly IMemoryCache _memoryCache;
    private readonly IHttpClientFactory _httpClientFactory;

    public CourseController(IMemoryCache memoryCache, IHttpClientFactory httpClientFactory)
    {
        _memoryCache = memoryCache;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<IActionResult> Index()
    {
        IReadOnlyList<Course> courses = _memoryCache.GetOrCreate(CoursesCacheKey, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
            return CreateCourses();
        })!;

        IReadOnlyList<JsonPlaceholderUser> users = Array.Empty<JsonPlaceholderUser>();
        string? usersError = null;

        try
        {
            var client = _httpClientFactory.CreateClient();
            users = await client.GetFromJsonAsync<List<JsonPlaceholderUser>>(
                "https://jsonplaceholder.typicode.com/users") ?? users;
        }
        catch (HttpRequestException)
        {
            usersError = "User information is temporarily unavailable. Please try again later.";
        }
        catch (TaskCanceledException)
        {
            usersError = "The user information request timed out. Please try again later.";
        }

        var model = new CourseIndexViewModel
        {
            Courses = courses,
            Users = users,
            UsersError = usersError,
            StudentName = Request.Cookies[StudentNameCookie],
            StudentEmail = Request.Cookies[StudentEmailCookie],
            SelectedCourseId = HttpContext.Session.GetInt32(SelectedCourseSessionKey)
        };

        return View(model);
    }

    [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "courseId" })]
    public IActionResult Details(int courseId)
    {
        var course = GetCourses().FirstOrDefault(item => item.CourseId == courseId);
        return course is null ? NotFound() : View(course);
    }

    public IActionResult MyCourse()
    {
        var selectedCourseId = HttpContext.Session.GetInt32(SelectedCourseSessionKey);
        var course = selectedCourseId.HasValue
            ? GetCourses().FirstOrDefault(item => item.CourseId == selectedCourseId.Value)
            : null;

        ViewData["StudentName"] = Request.Cookies[StudentNameCookie];
        ViewData["StudentEmail"] = Request.Cookies[StudentEmailCookie];
        return View(course);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Enroll(int courseId, string studentName, string studentEmail)
    {
        var course = GetCourses().FirstOrDefault(item => item.CourseId == courseId);
        if (course is null)
        {
            return NotFound();
        }

        if (string.IsNullOrWhiteSpace(studentName))
        {
            ModelState.AddModelError(nameof(studentName), "Please enter your name to enroll.");
        }

        if (string.IsNullOrWhiteSpace(studentEmail) ||
            !new System.ComponentModel.DataAnnotations.EmailAddressAttribute().IsValid(studentEmail))
        {
            ModelState.AddModelError(nameof(studentEmail), "Please enter a valid email address.");
        }

        if (!ModelState.IsValid)
        {
            ViewData["StudentName"] = studentName;
            ViewData["StudentEmail"] = studentEmail;
            return View("Details", course);
        }

        var normalizedName = studentName.Trim();
        var normalizedEmail = studentEmail.Trim();
        Response.Cookies.Append(StudentNameCookie, normalizedName, new CookieOptions
        {
            HttpOnly = true,
            IsEssential = true,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddDays(30)
        });
        Response.Cookies.Append(StudentEmailCookie, normalizedEmail, new CookieOptions
        {
            HttpOnly = true,
            IsEssential = true,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddDays(30)
        });
        HttpContext.Session.SetInt32(SelectedCourseSessionKey, course.CourseId);

        return View("EnrollmentSuccess", (course, normalizedName, normalizedEmail));
    }

    private IReadOnlyList<Course> GetCourses()
    {
        return _memoryCache.GetOrCreate(CoursesCacheKey, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
            return CreateCourses();
        })!;
    }

    private static IReadOnlyList<Course> CreateCourses()
    {
        return new List<Course>
        {
            new() { CourseId = 1, CourseName = "Full-Stack Web Development", Duration = "16 weeks", Fees = 1299m, DiscountPercentage = 15m },
            new() { CourseId = 2, CourseName = "Data Science with Python", Duration = "12 weeks", Fees = 1099m, DiscountPercentage = 10m },
            new() { CourseId = 3, CourseName = "Cloud Computing Fundamentals", Duration = "8 weeks", Fees = 799m, DiscountPercentage = 5m },
            new() { CourseId = 4, CourseName = "UI/UX Product Design", Duration = "10 weeks", Fees = 899m, DiscountPercentage = 12m }
        };
    }
}