namespace SevenAssignmentLibrary.Services
{
    public interface ICacheService
    {
        void Set<T>(string key, T obj, int mins);
        T Get<T>(string key);
    }
}