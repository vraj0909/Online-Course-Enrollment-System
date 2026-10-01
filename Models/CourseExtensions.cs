namespace CourseEnrollmentSystem.Models;

public static class CourseExtensions
{
    public static decimal CalculateFinalFees(this Course course)
    {
        return course.Fees * (1 - course.DiscountPercentage / 100);
    }
}