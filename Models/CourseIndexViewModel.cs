namespace CourseEnrollmentSystem.Models;

public class CourseIndexViewModel
{
    public IReadOnlyList<Course> Courses { get; init; } = Array.Empty<Course>();
    public IReadOnlyList<JsonPlaceholderUser> Users { get; init; } = Array.Empty<JsonPlaceholderUser>();
    public string? UsersError { get; init; }
    public string? StudentName { get; init; }
    public string? StudentEmail { get; init; }
    public int? SelectedCourseId { get; init; }
}