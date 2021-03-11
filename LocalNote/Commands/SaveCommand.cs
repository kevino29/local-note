using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;

namespace LocalNote.Commands
{
    public class SaveCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private ViewModels.NoteViewModel notes;

        public SaveCommand(ViewModels.NoteViewModel notes)
        {
            this.notes = notes;
        }

        public bool CanExecute(object parameter)
        {
            return notes.SelectedNote != null;
        }

        public async void Execute(object parameter)
        {
            Views.SaveNoteDialog save = new Views.SaveNoteDialog();
            ContentDialogResult result = await save.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {

            }
        }

        public void FireCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
