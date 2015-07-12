using SharedFx.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGTV.ViewModels
{
    public class MGBindableBase : BindableBase
    {
        private static string background = string.Empty;

        public string Background
        {
            get { return background; }
            set { SetProperty<string>(ref background, value); }
        }

        public MGBindableBase()
        {
        }
    }
}
