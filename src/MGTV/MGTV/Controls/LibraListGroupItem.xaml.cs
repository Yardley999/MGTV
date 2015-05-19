using MGTV.ViewModels;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MGTV.Controls
{
    public sealed partial class LibraListGroupItem : UserControl
    {
        public static readonly DependencyProperty DataListProperty = DependencyProperty.Register("DataList", typeof(ObservableCollection<Video>), typeof(LibraListGroupItem), new PropertyMetadata(null));

        public ObservableCollection<Video> DataList
        {
            get { return (ObservableCollection<Video>)GetValue(DataListProperty); }
            set { SetValue(DataListProperty, value); }
        }

        public LibraListGroupItem()
        {
            this.InitializeComponent();
            this.root.DataContext = this;
        }
    }
}
