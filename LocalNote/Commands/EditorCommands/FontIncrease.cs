using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LocalNote.Commands.EditorCommands {
    public class FontIncrease : ICommand {
        public event EventHandler CanExecuteChanged;
        private readonly ViewModels.NoteViewModel vm;

        public FontIncrease(ViewModels.NoteViewModel vm) {
            this.vm = vm;
        }

        public bool CanExecute(object parameter) {
            return true;
        }

        public void Execute(object parameter) {
            vm.Editor.Document.Selection.CharacterFormat.Size += 2;
        }

        public void FireCanExecuteChanged() {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
