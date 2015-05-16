using MGTV.Pages;
using MGTV.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MGTV.Controls
{
    public sealed partial class ItemTile : UserControl
    {

        public static readonly DependencyProperty DataProperty = DependencyProperty.Register("Data", typeof(Video), typeof(ItemTile), new PropertyMetadata(null));

        public Video Data
        {
            get { return (Video)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        #region Life Cycle

        public ItemTile()
        {
            this.InitializeComponent();
            this.root.DataContext = this;
        }

        #endregion

        #region Event

        private void ItemTile_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            VideoPlayPage.PageParams param = new VideoPlayPage.PageParams();
            param.VideoId = Data.VideoId;

            App.Instance.Frame.Navigate(typeof(VideoPlayPage), param);
        }

        #endregion
    }
}
