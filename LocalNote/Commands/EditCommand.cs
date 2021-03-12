using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LocalNote.Commands
{
    public class EditCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private ViewModels.NoteViewModel noteViewModel;

        public EditCommand(ViewModels.NoteViewModel noteViewModel)
        {
            this.noteViewModel = noteViewModel;
        }

        public bool CanExecute(object parameter)
        {
            if (this.noteViewModel.SelectedNote == null) return false;

            return !this.noteViewModel.EditMode;
        }

        public void Execute(object parameter)
        {
            this.noteViewModel.EditMode = true;
            FireCanExecuteChanged();
            this.noteViewModel.ReadOnly = false;
            this.noteViewModel.FirePropertyChanged("ReadOnly");
        }

        public void FireCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
