using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LocalNote.Commands
{
    public class DeleteCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private ViewModels.NoteViewModel noteViewModel;

        public DeleteCommand(ViewModels.NoteViewModel noteViewModel)
        {
            this.noteViewModel = noteViewModel;
        }

        public bool CanExecute(object parameter)
        {
            if (this.noteViewModel.SelectedNote == null ||
                this.noteViewModel.SelectedNote == this.noteViewModel.Buffer)
                return false;
            return true;
        }

        public void Execute(object parameter)
        {
            // Get the selected note
            Models.NoteModel noteToDelete = this.noteViewModel.SelectedNote;

            // Remove the selected note from the notes lists
            this.noteViewModel.Notes.Remove(noteToDelete);
            this.noteViewModel.NotesForLV.Remove(noteToDelete);
            this.noteViewModel.FirePropertyChanged("NotesForLV");

            // Set the selected note to null
            this.noteViewModel.SelectedNote = null;

            // Reset the notes properties
            this.noteViewModel.NoteTitle = "";
            this.noteViewModel.FirePropertyChanged("NoteTitle");
            this.noteViewModel.NoteContent = "";
            this.noteViewModel.FirePropertyChanged("NoteContent");

            // Notify that the selected note has changed
            this.noteViewModel.FirePropertyChanged("SelectedNote");

            // Delete the data file in the repo
            Repositories.NotesRepo.DeleteNoteFile(noteToDelete);
        }

        public void FireCanExecuteChanged()
        {
            this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
