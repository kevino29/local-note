
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LocalNote.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer;

namespace LocalNoteUnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [UITestMethod]
        public void Test_ReadOnly_WhenEditMode()
        {
            NoteViewModel vm = new NoteViewModel();
            vm.EditMode = true;

            // THIS IS WRONG! IT SHOULD BE FALSE!!!
            Assert.IsTrue(vm.ReadOnly);
        }
    }
}
