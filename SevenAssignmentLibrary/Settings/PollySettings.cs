namespace SevenAssignmentLibrary.Settings
{
    public class PollySettings
    {
        public int NumOfExceptionsAllowed { get; set; }

        public int DurationOfBreakInMins { get; set; }

        public int RetryCount { get; set; }

        public int RetrySeconds { get; set; }
    }
}
