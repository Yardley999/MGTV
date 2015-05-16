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
            //VideoPlayPage.PageParams param = new VideoPlayPage.PageParams();
            //param.PlayList.Add(new ViewModels.PlayListItem() {
            //    IsPlaying = true,
            //    Name = "第一集",
            //    Url = "http://pcfastvideo.imgo.tv/7aaf448ff2f6973ce4c64ec2eb0cba5a/555591e6/c1/2015/dianshiju/humamaoba/2015051279df128e-0e54-4bd8-8b5c-494c78681940.fhv/playlist.m3u8?uuid=a638db6be2314038a5bcaa2cc8170a4e"
            //});

            //param.PlayList.Add(new ViewModels.PlayListItem() {
            //    IsPlaying = false,
            //    Name = "第二集",
            //    Url = "http://wpc.866f.edgecastcdn.net/03866F/greyback/yourtrinity/130929-webword_,2500,1500,580,265,.mp4.m3u8"
            //});

            //App.Instance.Frame.Navigate(typeof(VideoPlayPage), param);
        }

        #endregion
    }
}
