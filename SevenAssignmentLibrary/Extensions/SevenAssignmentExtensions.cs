namespace SevenAssignmentLibrary.Extensions
{
    public static class SevenAssignmentExtensions
    {
        public static string GetGenderFullName(this string gender)
        {
            return gender.Replace("M", "Male")
                .Replace("F", "Female")
                .Replace("T", "Female")
                .Replace("Y", "Male");
        }
    }
}
