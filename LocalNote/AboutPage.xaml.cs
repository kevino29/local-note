using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace LocalNote {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AboutPage : Page {
        public AboutPage() {
            this.InitializeComponent();

            Package package = Package.Current;
            PackageId id = package.Id;
            version.Text = "v" + id.Version.Major + "." + id.Version.Minor + "." + id.Version.Build;
            title.Text = package.DisplayName;
            publisher.Text = package.PublisherDisplayName;
            copyright.Text = string.Format("{0} ©{1}", publisher.Text, DateTime.Now.Year);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = 
                AppViewBackButtonVisibility.Visible;

            SystemNavigationManager.GetForCurrentView().BackRequested += AboutPage_BackRequested;
        }

        private void AboutPage_BackRequested(object sender, BackRequestedEventArgs e) {
            if (Frame.CanGoBack)
                Frame.GoBack();
            e.Handled = true;
        }
    }
}
