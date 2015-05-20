using MGTV.Pages;
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
            this.Tapped += LibraryItemTile_Tapped;
        }

        private void LibraryItemTile_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            VideoPlayPage.PageParams para = new VideoPlayPage.PageParams();
            para.VideoId = Data.VideoId;

            App.Instance.Frame.Navigate(typeof(VideoPlayPage), para);
        }
    }
}
