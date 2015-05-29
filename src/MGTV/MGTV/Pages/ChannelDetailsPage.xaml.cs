using MGTV.MG.API;
using System.Linq;
using MGTV.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.Text;
using SharedFx.UI.Animations;
using System;

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

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            UnExpandFilter();
            hideFilterPanel.IsHitTestVisible = false;
        }

        #endregion

        #region Init

        private void Init()
        {
            viewModel = new ChannelDetailsPageViewModel();
            root.DataContext = viewModel;
            FilterPanelReset();
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
            page = 1;

            await ChannelAPI.GetLibraryList(data =>
            {
                viewModel.VideoCount = data.Count;
                viewModel.GroupList.Clear();

                if (data.Videos != null)
                {
                    RemoveLoadMoreControl();
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

                    if(data.Videos.Length == limit)
                    {
                        EnusureLoadMoreControl();
                    }
                }

                // lazy init
                //
                this.FindName("contentScrollViwer");
                this.FindName("videoCountText");
                page++;

                indicator.IsActive = false;

            }, error =>
            {
                //indicator.IsActive = false;
            },
            pageParams.ChannelId,
            orderType,
            filterDicts,
            page,
            limit);
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

        #region Load More

        int page = 1;
        int limit = 40;

        private VideoGroup loadMoreControlDataContext = new VideoGroup() { IsLoadMore = true };

        public void EnusureLoadMoreControl()
        {
            if (viewModel.GroupList != null && !viewModel.GroupList.Contains(loadMoreControlDataContext))
            {
                viewModel.GroupList.Add(loadMoreControlDataContext);
            }
        }

        public void RemoveLoadMoreControl()
        {
            if (viewModel.GroupList != null && viewModel.GroupList.Contains(loadMoreControlDataContext))
            {
                viewModel.GroupList.Remove(loadMoreControlDataContext);
            }
        }

        private async void loadMore_Loaded(object sender, RoutedEventArgs e)
        {
            if(indicator.IsActive)
            {
                return;
            }

            indicator.IsActive = true;
            await ChannelAPI.GetLibraryList(data => {

                if (data.Videos != null)
                {
                    RemoveLoadMoreControl();
                    int baseIndex = viewModel.GroupList.Count;

                    for (int i = 0; i < data.Videos.Length; i++)
                    {
                        int groupIndex = baseIndex + (i / 10);
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

                    if (data.Videos.Length == limit)
                    {
                        EnusureLoadMoreControl();
                        page++;
                    }

                    indicator.IsActive = false;
                }

            }, error => {
                indicator.IsActive = false;
            },
            pageParams.ChannelId,
            GetOrderType(),
            GetFilters(),
            page,
            limit);
        }

        #endregion

        #region Event

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        #region Filters

        private bool isFilterAniRuning = false;
        private bool isExpanded = false;
        private TimeSpan filterAnimationDuration = TimeSpan.FromSeconds(0.167);

        private void FilterPanelReset()
        {
            isFilterAniRuning = false;
            MoveAnimation.MoveBy(this.filterPanel, filterPanel.Width, 0, TimeSpan.FromSeconds(0), null);
            this.showFilterText.Opacity = 1;
        }

        private void ExpandFilter()
        {
            if (isFilterAniRuning || isExpanded)
            {
                return;
            }
            isFilterAniRuning = true;
            
            MoveAnimation.MoveBy(this.filterPanel, -filterPanel.Width, 0, filterAnimationDuration, fe => {
                isFilterAniRuning = false;
                isExpanded = true;
                hideFilterPanel.IsHitTestVisible = true;
            });
        }

        private void UnExpandFilter()
        {
            if (isFilterAniRuning
                || !isExpanded)
            {
                return;
            }
            isFilterAniRuning = true;
            
            MoveAnimation.MoveBy(this.filterPanel, filterPanel.ActualWidth, 0, filterAnimationDuration, fe => {
                isFilterAniRuning = false;
                isExpanded = false;
                hideFilterPanel.IsHitTestVisible = false;
            });
        }

        private void ShowFilter_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            ExpandFilter();
        }

        private void HideFilter_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            UnExpandFilter();
        }

        private async void ChangeFilter_Click(object sender, RoutedEventArgs e)
        {
            await LoadLibraryDataAsync(GetFilters(), GetOrderType());
        }

        private Dictionary<string, string> GetFilters()
        {
            Dictionary<string, string> filters = new Dictionary<string, string>();
            foreach (var FilterGroup in viewModel.Filters)
            {
                string key = FilterGroup.Type;
                var filterValue = FilterGroup.Values.FirstOrDefault(v => v.IsChecked);
                if (filterValue == null)
                {
                    continue;
                }

                if (!filters.ContainsKey(key))
                {
                    filters.Add(key, filterValue.Id);
                }
            }

            return filters;
        }

        private OrderType GetOrderType()
        {
            OrderType orderType = OrderType.LASTEST;
            if (this.lastestRadioBtn.IsChecked == true)
            {
                orderType = OrderType.HOT;
            }

            return orderType;
        }

        #endregion

        #endregion

        #region App Bar

        private void TopAppBar_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            e.Handled = true;

            if (this.topAppBar.IsOpen)
            {
                this.topAppBar.IsOpen = false;
            }
        }

        #endregion
    }
}
