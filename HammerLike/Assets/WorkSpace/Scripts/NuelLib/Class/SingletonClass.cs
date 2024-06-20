
namespace NuelLib
{
    public class SingletonClass<T> where T : SingletonClass<T>, new()
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new T();
                    instance.Initialize();
                }
                return instance;
            }
        }

        // Override this method to initialize the singleton instance
        protected virtual void Initialize()
        {
            // Do any necessary initialization here
        }
    }
}