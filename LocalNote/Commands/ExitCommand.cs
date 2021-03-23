using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace LocalNote.Commands
{
    public class ExitCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private readonly ViewModels.NoteViewModel noteViewModel;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="noteViewModel"></param>
        public ExitCommand(ViewModels.NoteViewModel noteViewModel)
        {
            this.noteViewModel = noteViewModel;
        }

        /// <summary>
        /// Returns true if the command can be executed. Returns false otherwise.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="parameter"></param>
        public async void Execute(object parameter)
        {
            // Check if there are any unsaved changes
            foreach (var note in noteViewModel.Notes)
            {
                if (note.NeedSaving)
                {
                    // If there is an unsaved note, create a dialog for confirmation
                    ContentDialog dialog = new ContentDialog()
                    {
                        Title = "Unsaved Changes",
                        Content = "There are note(s) that you haven't saved yet. Continue?",
                        PrimaryButtonText = "Yes",
                        SecondaryButtonText = "No",
                    };
                    var result = await dialog.ShowAsync();

                    if (result == ContentDialogResult.Primary) break;
                    else return; // Skip the exit when the user clicks 'Cancel'
                }
            }

            // Exit the application
            try
            {
                Application.Current.Exit();
            }
            catch (Exception)
            {
                Debug.WriteLine("There's an error while closing the program.");
            }
        }

        /// <summary>
        /// Fires the CanExecuteChanged event.
        /// </summary>
        public void FireCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
