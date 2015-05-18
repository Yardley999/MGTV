using MGTV.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MGTV.Controls
{
    public sealed partial class LibraryItemTile : UserControl
    {
        public static readonly DependencyProperty DataProperty = DependencyProperty.Register("Data", typeof(Video), typeof(LibraryItemTile), new PropertyMetadata(null));

        public Video Data
        {
            get { return (Video)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        public LibraryItemTile()
        {
            this.InitializeComponent();
            this.root.DataContext = this;
        }
    }
}
