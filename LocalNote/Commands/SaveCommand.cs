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
        private List<string> savedNoteTitles = new List<string>();

        public SaveCommand(ViewModels.NoteViewModel noteViewModel)
        {
            this.noteViewModel = noteViewModel;
        }

        public bool CanExecute(object parameter)
        {
            if (noteViewModel.SelectedNote == null) return false;
            return noteViewModel.SelectedNote.NeedSaving;
        }

        public async void Execute(object parameter)
        {
            // Check if the note has just been created
            if (noteViewModel.SelectedNote.Title == "Untitled Note")
            {
                // Create a save note dialog
                Views.SaveNoteDialog save = new Views.SaveNoteDialog();
                ContentDialogResult result;

                // This loop makes sure that the user doesn't enter
                // a duplicate title for a note
                while (true)
                {
                    bool flag = false;
                    result = await save.ShowAsync();

                    foreach(var note in noteViewModel.Notes)
                    {
                        if (note.Title == save.NoteTitle)
                        {
                            flag = true;
                            break;
                        }
                    }

                    if (flag)
                    {
                        ContentDialog error = new ContentDialog()
                        {
                            Title = "Error Occurred",
                            Content = "That title already exists. Please enter a new unique title.",
                            PrimaryButtonText = "OK",
                        };
                        await error.ShowAsync();
                    }
                    else break;
                }

                if (result == ContentDialogResult.Primary)
                {
                    // Get the new note title from the dialog
                    noteViewModel.SelectedNote.Title = save.NoteTitle;

                    // Notify that the selected note and the selected note's title changed
                    noteViewModel.FirePropertyChanged("SelectedNote");
                    noteViewModel.SelectedNote.FirePropertyChanged("Title");
                }
                // Do not continue if the user clicks 'Cancel'
                else return;
            }

            // Add the note title to the list of saved notes
            if (!savedNoteTitles.Contains(noteViewModel.SelectedNote.Title))
            {
                savedNoteTitles.Add(noteViewModel.SelectedNote.Title);
            }

            // Save the note data to file
            Repositories.NotesRepo.SaveNoteToFile(noteViewModel.SelectedNote);

            // Show a dialog that the save was successful
            ContentDialog dialog = new ContentDialog()
            {
                Title = "Saved successfully",
                Content = "'" + noteViewModel.SelectedNote.Title + "'" + " has been saved.",
                PrimaryButtonText = "OK",
            };
            await dialog.ShowAsync();

            // Turn off need saving
            this.noteViewModel.SelectedNote.NeedSaving = false;
            this.noteViewModel.SelectedNote.FirePropertyChanged("NeedSaving");
            this.noteViewModel.SaveCommand.FireCanExecuteChanged();

            // Turn off edit mode
            this.noteViewModel.EditMode = false;
            this.noteViewModel.FirePropertyChanged("EditMode");
            this.noteViewModel.EditCommand.FireCanExecuteChanged();

            // Turn on read only mode
            this.noteViewModel.ReadOnly = true;
            this.noteViewModel.FirePropertyChanged("ReadOnly");
        }

        public void FireCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
