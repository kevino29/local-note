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
        private ViewModels.NoteViewModel noteViewModel;

        public SaveCommand(ViewModels.NoteViewModel noteViewModel)
        {
            this.noteViewModel = noteViewModel;
        }

        public bool CanExecute(object parameter)
        {
            return noteViewModel.SelectedNote != null;
        }

        public async void Execute(object parameter)
        {
            // Check if the note has been saved once, and in the repo
            if (noteViewModel.SelectedNote.NoteTitle == "Untitled Note")
            {
                // Make new data in the repo
                Views.SaveNoteDialog save = new Views.SaveNoteDialog();
                ContentDialogResult result = await save.ShowAsync();

                if (result == ContentDialogResult.Primary)
                {
                    noteViewModel.SelectedNote.NoteTitle = save.NoteTitle;
                    Repositories.NotesRepo.SaveNotesToFile(noteViewModel.SelectedNote);

                    ContentDialog dialog = new ContentDialog()
                    {
                        Title = "Saved successfully",
                        Content = "'" + noteViewModel.SelectedNote.NoteTitle + "'" + " has been saved.",
                        PrimaryButtonText = "OK",
                    };
                    await dialog.ShowAsync();
                }
            }
            // The note is already in the repo, just override the data
            else
            {
                // Override existing data in the repo
            }
        }

        public void FireCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
