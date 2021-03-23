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

        public AboutCommand()
        {
            about = new Views.AboutDialog();
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public async void Execute(object parameter)
        {
            await about.ShowAsync();
        }
    }
}
