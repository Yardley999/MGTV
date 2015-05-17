﻿using SharedFx.Data;
using System.Collections.ObjectModel;

namespace MGTV.ViewModels
{
    public class MainPageViewModel : BindableBase
    {
        private Category recommendation;

        public Category Recommendation
        {
            get { return recommendation; }
            set { SetProperty<Category>(ref recommendation, value); }
        }

        public ObservableCollection<Category> Categories { get; set; }

        public ObservableCollection<Channel> ChannelNavigationItems { get; set; }

        public MainPageViewModel()
        {
            recommendation = new Category();
            Categories = new ObservableCollection<Category>();
            ChannelNavigationItems = new ObservableCollection<Channel>();
            ChannelNavigationItems.Add(Channel.Home);
        }
    }
}
