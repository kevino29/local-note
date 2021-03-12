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
        private readonly ViewModels.NoteViewModel noteViewModel;

        public EditCommand(ViewModels.NoteViewModel noteViewModel)
        {
            this.noteViewModel = noteViewModel;
        }

        public bool CanExecute(object parameter)
        {
            // Always false if there are no notes selected
            if (this.noteViewModel.SelectedNote == null) return false;

            return !this.noteViewModel.EditMode;
        }

        public void Execute(object parameter)
        {
            // Change the edit mode to true, then notify
            this.noteViewModel.EditMode = true;
            FireCanExecuteChanged();

            // Change the read only mode to false, then notify
            this.noteViewModel.ReadOnly = false;
            this.noteViewModel.FirePropertyChanged("ReadOnly");
        }

        public void FireCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
