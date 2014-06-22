using Caliburn.Micro;
using Mimicka.Models;

namespace Mimicka.ViewModels
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
