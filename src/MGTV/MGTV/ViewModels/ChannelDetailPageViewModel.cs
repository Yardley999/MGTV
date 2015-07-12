using SharedFx.Data;
using SharedFx.UI;
using System.Collections.ObjectModel;
using System;

namespace MGTV.ViewModels
{
    public class ChannelDetailsPageViewModel : MGBindableBase
    {
        private string title;

        public string Title
        {
            get { return title; }
            set { SetProperty<string>(ref title, value); }
        }

        private int videoCount;

        public int VideoCount 
        {
            get { return videoCount; }
            set { SetProperty<int>(ref videoCount, value); }
        }

        public ObservableCollection<Filter> Filters { get; set; }

        public ObservableCollection<VideoGroup> GroupList { get; set; }

        public ChannelDetailsPageViewModel()
        {
            title = string.Empty;
            videoCount = 0;
            Filters = new ObservableCollection<Filter>();
            GroupList = new ObservableCollection<VideoGroup>();
        }
    }

    public class VideoGroup : BindableBase, ILoadMoreItem
    {
        public ObservableCollection<Video> Group { get; set; }

        public bool IsLoadMore { get; set; }

        public VideoGroup()
        {
            Group = new ObservableCollection<Video>();
            IsLoadMore = false;
        }
    }

}