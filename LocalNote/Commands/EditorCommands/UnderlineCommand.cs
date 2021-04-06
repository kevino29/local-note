using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LocalNote.Commands.EditorCommands {
    public class UnderlineCommand : ICommand {
        public event EventHandler CanExecuteChanged;
        private readonly ViewModels.NoteViewModel vm;

        public UnderlineCommand(ViewModels.NoteViewModel vm) {
            this.vm = vm;
        }

        public bool CanExecute(object parameter) {
            return true;
        }

        public void Execute(object parameter) {
            if (vm.Editor.Document.Selection.CharacterFormat.Underline == Windows.UI.Text.UnderlineType.None)
                this.vm.Editor.Document.Selection.CharacterFormat.Underline = Windows.UI.Text.UnderlineType.Single;
            else
                this.vm.Editor.Document.Selection.CharacterFormat.Underline = Windows.UI.Text.UnderlineType.None;
        }

        public void FireCanExecuteChanged() {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
