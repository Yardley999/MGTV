﻿using MGTV.MG.API;
using MGTV.Pages;
using MGTV.ViewModels;
using SharedFx.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MGTV.Controls
{
    public sealed partial class ChannelZone : UserControl
    {
        private static ObservableCollection<Channel> ChannelNavigationItems;

        public ChannelZone()
        {
            this.InitializeComponent();
            TopAppBarItemListDataBinding();
        }

        #region NavigationBar

        private Dictionary<int, string> AppBarIconMap = new Dictionary<int, string>() {
            { 1, "ms-appx:///Assets/NavListIcon/ico-nav-02.png"},
            { 2, "ms-appx:///Assets/NavListIcon/ico-nav-03.png"},
            { 3, "ms-appx:///Assets/NavListIcon/ico-nav-04.png"},
            { 4, "ms-appx:///Assets/NavListIcon/ico-nav-06.png"},
            { 5, "ms-appx:///Assets/NavListIcon/ico-nav-07.png"},
            { 6, "ms-appx:///Assets/NavListIcon/ico-nav-05.png"},
            { 7, "ms-appx:///Assets/NavListIcon/ico-nav-08.png"},
            { 8, "ms-appx:///Assets/NavListIcon/ico-nav-09.png"},
            { 9, "ms-appx:///Assets/NavListIcon/ico-nav-10.png"},
            {99, "ms-appx:///Assets/NavListIcon/ico-nav-10.png" }
        };

        private void TopAppBarItemListDataBinding()
        {
            if (ChannelNavigationItems == null)
            {
                ChannelNavigationItems = new ObservableCollection<Channel>();
                ChannelNavigationItems.Add(Channel.Live);
            }

            chanelZoneItemList.ItemsSource = ChannelNavigationItems;
        }

        public async Task LoadTopAppBarItemsAsync()
        {
            await ChannelAPI.GetMajorList(channels =>
            {
                ChannelNavigationItems.Clear();
                //ChannelNavigationItems.Add(Channel.Home);

                if (channels != null)
                {
                    foreach (var item in channels)
                    {
                        string iconUrl = item.IconUrl;
                        if (string.IsNullOrEmpty(iconUrl))
                        {
                            if (AppBarIconMap.ContainsKey(item.Id))
                            {
                                iconUrl = AppBarIconMap[item.Id];
                            }
                        }

                        ChannelNavigationItems.Add(new Channel()
                        {
                            Id = item.Id,
                            IconUrl = iconUrl,
                            Name = item.Name
                        });
                    }
                }
                ChannelNavigationItems.Add(Channel.Live);

            }, error =>
            {

            });
        }

        private void ChannelZoneItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var dataContext = sender.GetDataContext<Channel>();

            if (dataContext.Id > 0)
            {
                ChannelDetailsPage.PageParams para = new ChannelDetailsPage.PageParams();
                para.ChannelId = dataContext.Id;
                para.ChannelName = dataContext.Name;

                App.Instance.Frame.Navigate(typeof(ChannelDetailsPage), para);
            }
        }

        #endregion
    }
}
