using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MGTV.Controls
{
    public sealed partial class LocalTile : UserControl
    {
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(LocalTile), new PropertyMetadata(""));
        public static readonly DependencyProperty CenterImageUriProperty = DependencyProperty.Register("CenterImageUri", typeof(Uri), typeof(LocalTile), new PropertyMetadata(new Uri("ms-appx:///Assets/NavListIcon/ico-nav-10.png")));

        #region Properties 

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public Uri CenterImageUri
        {
            get { return (Uri)GetValue(CenterImageUriProperty); }
            set
            {
                SetValue(CenterImageUriProperty, value);
                BitmapImage bmp = new BitmapImage();
                bmp.UriSource = value;
                CenterImage.Source = bmp;
            }
        }

        #endregion

        public LocalTile()
        {
            this.InitializeComponent();
            this.root.DataContext = this;
        }
    }
}
