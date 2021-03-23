using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LocalNote.Commands
{
    public class AboutCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private readonly Views.AboutDialog about;

        /// <summary>
        /// Constructor
        /// </summary>
        public AboutCommand()
        {
            about = new Views.AboutDialog();
        }

        /// <summary>
        /// Always returns true.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        /// Executes the about command.
        /// </summary>
        /// <param name="parameter"></param>
        public async void Execute(object parameter)
        {
            await about.ShowAsync();
        }
    }
}
