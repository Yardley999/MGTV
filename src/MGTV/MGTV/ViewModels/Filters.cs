using SharedFx.Data;
using System.Collections.ObjectModel;

namespace MGTV.ViewModels
{
    public class Filter : BindableBase
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { SetProperty<string>(ref name, value); }
        }

        private string type;

        public string Type
        {
            get { return type; }
            set { SetProperty<string>(ref type, value); }
        }


        public ObservableCollection<FilterValue> Values { get; set; }

        public Filter()
        {
            Values = new ObservableCollection<FilterValue>();
            name = string.Empty;
            type = string.Empty;
        }
    }

    public class FilterValue : BindableBase
    {
        private bool isChecked;

        public bool IsChecked
        {
            get { return isChecked; }
            set { SetProperty<bool>(ref isChecked, value); }
        }

        private string val;

        public string ValueOfFilter
        {
            get { return val; }
            set { SetProperty<string>(ref val, value); }
        }

        private string id;

        public string Id
        {
            get { return id; }
            set { SetProperty<string>(ref id, value); }
        }


        public FilterValue()
        {
            val = string.Empty;
            isChecked = false;
            id = string.Empty;
        }
    }
}
