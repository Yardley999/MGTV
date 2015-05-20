using MGTV.MG.API;
using MGTV.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MGTV.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
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

        PageParams pageParams;
        ChannelDetailsPageViewModel viewModel;

        public ChannelDetailsPage()
        {
            this.InitializeComponent();
            Init();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if(e.NavigationMode == NavigationMode.Back)
            {
                return;
            }

            pageParams = e.Parameter as PageParams;

            if (pageParams != null)
            {
                viewModel.Background = pageParams.Background;
                viewModel.Title = pageParams.ChannelName;
                LoadFilterDataAsync();
                LoadLibraryDataAsync(null);
            }
        }

        private void Init()
        {
            viewModel = new ChannelDetailsPageViewModel();
            root.DataContext = viewModel;
        }

        private async Task LoadLibraryDataAsync(Dictionary<string, string> filterDicts)
        {
            if(pageParams == null)
            {
                return;
            }

            indicator.IsActive = true;

            await ChannelAPI.GetLibraryList(data => {
                viewModel.VideoCount = data.Count;
                viewModel.GroupList.Clear();

                if(data.Videos != null)
                {
                    for (int i = 0; i < data.Videos.Length; i++)
                    {
                        int groupIndex = i / 10;
                        if(viewModel.GroupList.Count > groupIndex)
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

                indicator.IsActive = false;

            }, error => {
                indicator.IsActive = false;
            },
            pageParams.ChannelId, 
            OrderType.LASTEST, 
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

            await ChannelAPI.GetLibraryFilters(pageParams.ChannelId, filters => {
                if (filters != null)
                {
                    viewModel.Filters.Clear();
                    foreach (var filter in filters)
                    {
                        Filter f = new Filter();
                        f.Name = filter.Name;
                        f.Type = filter.Type;

                        if(filter.Items != null)
                        {
                            foreach (var val in filter.Items)
                            {
                                string name = val.Name;

                                if(string.IsNullOrEmpty(name))
                                {
                                    name = "其他";
                                }

                                f.Values.Add(new FilterValue() {
                                    ValueOfFilter = name,
                                    IsChecked = false,
                                    Id = val.Id
                                });
                            }
                        }

                        viewModel.Filters.Add(f);
                    }

                    this.FindName("filterPanel");
                }
            }, error => {
            });
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }
    }
}
