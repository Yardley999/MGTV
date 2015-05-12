using MGTV.Pages;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MGTV.Controls
{
    public sealed partial class ItemTile : UserControl
    {

        #region Dependency Property

        public static readonly DependencyProperty IntroProperty = DependencyProperty.Register("Intro", typeof(string), typeof(ItemTile), new PropertyMetadata(string.Empty));
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(ItemTile), new PropertyMetadata(string.Empty));
        public static readonly DependencyProperty ImageProperty = DependencyProperty.Register("Image", typeof(string), typeof(ItemTile), new PropertyMetadata(string.Empty));

        #endregion

        #region CLR Wrapper

        public string Image
        {
            get { return (string)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public string Intro
        {
            get { return (string)GetValue(IntroProperty); }
            set { SetValue(IntroProperty, value); }
        }

        #endregion


        public ItemTile()
        {
            this.InitializeComponent();
            this.DataContext = this;
            this.Tapped += ItemTile_Tapped;
            Test();
        }

        private void ItemTile_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            VideoPlayPage.PageParams param = new VideoPlayPage.PageParams();
            param.Url = "http://pchlsws1.imgo.tv/jiankong.fhv/playlist.m3u8";

            App.Instance.Frame.Navigate(typeof(VideoPlayPage), param);
        }

        private void Test()
        {
            this.Image = "ms-appx:///Assets/test.jpg";
            this.Intro = "爸爸去哪里了啊unalilaj阿斯顿法定";
            this.Title = "爸爸去哪儿了";
        }
    }
}
