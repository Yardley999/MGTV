using MGTV.MG.API;
using System.Linq;
using MGTV.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.Text;

namespace MGTV.Pages
{
    public sealed partial class ChannelDetailsPage : Page
    {
        #region Page Params

        public class PageParams
        {
            public string Background { get; set; }

            public int ChannelId { get; set; }

            public string ChannelName { get; set; }

            public PageParams()
            {
                Background = string.Empty;
                ChannelName = string.Empty;
                ChannelId = -1;
            }
        }

        #endregion

        #region Field && Property

        PageParams pageParams;
        ChannelDetailsPageViewModel viewModel;

        #endregion

        #region Life Cycle

        public ChannelDetailsPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Required;
            Init();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.NavigationMode == NavigationMode.Back)
            {
                return;
            }

            pageParams = e.Parameter as PageParams;

            if (pageParams != null)
            {
                Init();
                viewModel.Background = App.Instance.BackgroundImage;
                viewModel.Title = pageParams.ChannelName;
                LoadFilterDataAsync();
                LoadLibraryDataAsync(null);
            }
        }

        #endregion

        #region Init

        private void Init()
        {
            viewModel = new ChannelDetailsPageViewModel();
            root.DataContext = viewModel;
        }

        #endregion

        #region Data

        private async Task LoadLibraryDataAsync(Dictionary<string, string> filterDicts, OrderType orderType = OrderType.LASTEST)
        {
            if (pageParams == null)
            {
                return;
            }

            indicator.IsActive = true;

            await ChannelAPI.GetLibraryList(data =>
            {
                viewModel.VideoCount = data.Count;
                viewModel.GroupList.Clear();

                if (data.Videos != null)
                {
                    for (int i = 0; i < data.Videos.Length; i++)
                    {
                        int groupIndex = i / 10;
                        if (viewModel.GroupList.Count > groupIndex)
                        {
                            var group = viewModel.GroupList[groupIndex];
                            group.Group.Add(CopyVideoData(data.Videos[i]));
                        }
                        else
                        {
                            VideoGroup group = new VideoGroup();
                            group.Group.Add(CopyVideoData(data.Videos[i]));
                            viewModel.GroupList.Add(group);
                        }
                    }
                }

                // lazy init
                //
                this.FindName("contentScrollViwer");
                this.FindName("videoCountText");
                this.FindName("filterPanel");

                indicator.IsActive = false;

            }, error =>
            {
                //indicator.IsActive = false;
            },
            pageParams.ChannelId,
            orderType,
            filterDicts,
            1,
            40);
        }

        private Video CopyVideoData(MG.DataModels.Video data)
        {
            return new Video()
            {
                ImageUrl = data.ImageUrl,
                Intro = data.Desc,
                Name = data.Title,
                PlayCount = data.PlayCount,
                VideoId = data.Id
            };
        }

        private async Task LoadFilterDataAsync()
        {
            if (pageParams == null)
            {
                return;
            }

            await ChannelAPI.GetLibraryFilters(pageParams.ChannelId, filters =>
            {
                if (filters != null)
                {
                    viewModel.Filters.Clear();
                    foreach (var filter in filters)
                    {
                        Filter f = new Filter();
                        f.Name = filter.Name;
                        f.Type = filter.Type;

                        if (filter.Items != null)
                        {
                            foreach (var val in filter.Items)
                            {
                                string name = val.Name;

                                if (string.IsNullOrEmpty(name))
                                {
                                    name = "其他";
                                }

                                f.Values.Add(new FilterValue()
                                {
                                    ValueOfFilter = name,
                                    IsChecked = false,
                                    Id = val.Id
                                });
                            }
                        }
                        if(f.Values.Count > 0)
                        {
                            f.Values[0].IsChecked = true;
                        }

                        viewModel.Filters.Add(f);
                    }

                }
            }, error =>
            {
            });
        }

        #endregion

        #region Event

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private async void ChangeFilter_Click(object sender, RoutedEventArgs e)
        {
            OrderType orderType = OrderType.LASTEST;
            if (this.lastestRadioBtn.IsChecked == true)
            {
                orderType = OrderType.HOT;
            }

            Dictionary<string, string> filters = new Dictionary<string, string>();
            foreach (var FilterGroup in viewModel.Filters)
            {
                string key = FilterGroup.Type;
                var filterValue = FilterGroup.Values.FirstOrDefault(v => v.IsChecked);
                if(filterValue == null)
                {
                    continue;
                }

                if(!filters.ContainsKey(key))
                {
                    filters.Add(key, filterValue.Id);
                }
            }

            await LoadLibraryDataAsync(filters, orderType);
        }

        #endregion
    }
}
