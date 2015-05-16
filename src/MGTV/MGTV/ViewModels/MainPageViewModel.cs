using SharedFx.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public ObservableCollection<Category> Categoyies { get; set; }

        public MainPageViewModel()
        {
            recommendation = new Category();
            Categoyies = new ObservableCollection<Category>();
        }
    }
}
