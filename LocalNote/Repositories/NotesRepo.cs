using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace LocalNote.Repositories
{
    public class NotesRepo
    {
        private static StorageFolder notesFolder = ApplicationData.Current.LocalFolder;

        public async static void SaveNotesToFile(Models.NoteModel selected)
        {
            string fileName = selected.NoteTitle.Replace(" ", "_") + ".txt";
            try
            {
                StorageFile noteFile = 
                    await notesFolder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);
                await FileIO.AppendTextAsync(noteFile, selected.NoteContent);
                Debug.WriteLine(notesFolder.Path);
            }
            catch (Exception)
            {
                Debug.WriteLine("An error occured when saving the note file.");
            }
        }
    }
}
