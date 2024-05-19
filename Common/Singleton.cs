namespace Common
{
    public class Singleton<T> where T : new()
    {
        private static T? _instance;
        private static readonly object _lock = new();

        public static T Instance
        {
            get
            {
                lock (_lock)
                {
                    return _instance ??= new T();
                }
            }
        }
    }
}
