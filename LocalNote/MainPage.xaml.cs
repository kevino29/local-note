using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace LocalNote {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page {
        public ViewModels.NoteViewModel NoteViewModel { get; set; }
        public Commands.AboutCommand AboutCommand { get; }
        public MainPage() {
            this.InitializeComponent();
            this.NoteViewModel = new ViewModels.NoteViewModel(editor);
            this.AboutCommand = new Commands.AboutCommand(this);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                AppViewBackButtonVisibility.Collapsed;
        }

        private void Editor_TextChanged(object sender, RoutedEventArgs e) {
            editor.Document.GetText(Windows.UI.Text.TextGetOptions.FormatRtf, out string text);
            NoteViewModel.NoteContent = text;
            Debug.WriteLine(text);
        }
    }
}
