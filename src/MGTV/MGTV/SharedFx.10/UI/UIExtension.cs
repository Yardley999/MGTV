using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

public static class UIExtension
{
    public static T GetParent<T>(this FrameworkElement element) where T : FrameworkElement
    {
        if (element == null)
        {
            return null;
        }

        var parent = VisualTreeHelper.GetParent(element);

        if (parent == null)
        {
            return null;
        }

        while (parent != null)
        {
            if (parent is T)
            {
                return parent as T;
            }
            else
            {
                parent = VisualTreeHelper.GetParent(parent);
            }
        }

        return null;
    }

    public static T GetChildInVisualTree<T>(this FrameworkElement element) where T : FrameworkElement
    {
        if(element == null)
        {
            return null;
        }

        int count = VisualTreeHelper.GetChildrenCount(element);
        if(count > 0)
        {
            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(element, i);
                if (child is T)
                {
                    return child as T;
                }
                else
                {
                    if(child != null)
                    {

                        var childFrameworkElement = child as FrameworkElement;
                        if (childFrameworkElement != null)
                        {
                            var childInChildFE = childFrameworkElement.GetChildInVisualTree<T>();
                            if(childInChildFE != null)
                            {
                                return childInChildFE;
                            }
                        }
                    }
                }
            }
        }

        return null;
    }
}
