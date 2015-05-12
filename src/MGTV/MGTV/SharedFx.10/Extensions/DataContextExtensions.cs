using Windows.UI.Xaml;

namespace SharedFx.Extensions
{
    public static class DataContextExtensions
    {
        public static object GetDataContext(this object obj)
        {
            return (obj as FrameworkElement).DataContext;
        }

        public static T GetDataContext<T>(this object obj) where T : class
        {
            T context = (obj as FrameworkElement).DataContext as T;
            return context;
        }
    }
}
