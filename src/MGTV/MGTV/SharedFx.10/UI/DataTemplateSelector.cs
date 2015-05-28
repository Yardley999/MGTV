using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SharedFx.UI
{
    #region DateTemplateSelectorBase

    public abstract class DataTemplateSelector : ContentControl
    {
        public virtual DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return null;
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
            ContentTemplate = SelectTemplate(newContent, this);
        }
    }

    #endregion

    #region LoadMoreTemplateSelector

    public class LoadMoreDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ItemTemplate { get; set; }

        public DataTemplate LoadMoreTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            ILoadMoreItem itemdata = item as ILoadMoreItem;
            if (itemdata != null)
            {
                if (itemdata.IsLoadMore)
                {
                    return LoadMoreTemplate;
                }
                else
                {
                    return ItemTemplate;
                }
            }

            return base.SelectTemplate(item, container);
        }
    }

    public interface ILoadMoreItem
    {
        bool IsLoadMore { get; set; }
    }

    #endregion

    #region MultiTemplateSelector


    public class MultiTemplateSelector : DataTemplateSelector
    {
        public DataTemplate HeaderTemplate { get; set; }

        public DataTemplate ItemTemplate { get; set; }

        public DataTemplate LoadMoreTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            IMultipleTemplateItem itemData = item as IMultipleTemplateItem;
            if(itemData != null)
            {
                switch (itemData.TemplateType)
                {
                    case TemplateType.HEADER:
                        return HeaderTemplate;
                    case TemplateType.LOADMORE:
                        return LoadMoreTemplate;
                    case TemplateType.NORMAL:
                        return ItemTemplate;
                    default:
                        break;
                }
            }
            return base.SelectTemplate(item, container);
        }
    }

    public interface IMultipleTemplateItem
    {
        TemplateType TemplateType {get; set;}
    }

    public enum TemplateType
    {
        NORMAL,
        LOADMORE,
        HEADER
    }

    #endregion
}
