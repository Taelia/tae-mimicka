using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Caliburn.Micro;
using Tomestone.Models;
using System.Windows.Threading;

namespace Tomestone.ViewModels
{
    public class MainViewModel : PropertyChangedBase
    {
        private Main _model;

        //This is the start of the application.
        public MainViewModel()
        {
            _model = new Main(this);
        }
    }
}
