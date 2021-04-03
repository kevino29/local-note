using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;

namespace LocalNote.Commands {
    public class AboutCommand : ICommand {
        public event EventHandler CanExecuteChanged;
        private readonly Views.AboutDialog about;
        private Page page;

        /// <summary>
        /// Constructor
        /// </summary>
        public AboutCommand(Page page) {
            this.page = page;
            about = new Views.AboutDialog();
        }

        /// <summary>
        /// Always returns true.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter) {
            return true;
        }

        /// <summary>
        /// Executes the about command.
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter) {
            //await about.ShowAsync();
            page.Frame.Navigate(typeof(AboutPage));
        }

        /// <summary>
        /// Firest the CanExecuteChanged event.
        /// </summary>
        public void FireCanExecuteChanged() {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
