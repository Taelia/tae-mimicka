using System;
using System.Windows;
using TomeLib.Irc;

namespace Mimicka
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //First method to be called on the App's startup.
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            this.ShutdownMode = ShutdownMode.OnMainWindowClose;
            this.Exit += App_Exit;
        }

        void App_Exit(object sender, ExitEventArgs e)
        {
            Environment.Exit(1);
        }
    }
}
