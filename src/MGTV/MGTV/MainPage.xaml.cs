using MGTV.MG.API;
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
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MGTV
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            //ChannelAPI.GetList(9, list => {
            //    int i = list.Length;
            //}, error => {
            //    System.Diagnostics.Debug.WriteLine(error.Message);
            //});
            Action a = async () =>
               {

                   string address = await VideoAPI.GetRealVideoAddress("http://pcvcr.cdn.imgo.tv/ncrs/vod.do?fid=E87BA921D747F5B490568D78F239741F&limitrate=1292&file=%2Fc1%2F2015%2Fdianshiju%2Fhumamaoba%2F20150512bcb33d76-e505-4dd3-89ca-31c1ddf2d973.fhv&fmt=2&pno=6&m3u8=1&random=1431671081", null);
                   string a2343 = address;
               };

            a();
        }

        private void RootGrid_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            this.topMenuBar.Visibility = Visibility.Visible;
        }

        private void ContentGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.topMenuBar.Visibility = Visibility.Collapsed;
            e.Handled = true;
        }
    }
}
