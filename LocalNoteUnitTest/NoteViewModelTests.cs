
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LocalNote.ViewModels;

namespace LocalNoteUnitTest
{
    [TestClass]
    public class NoteViewModelTests
    {
        private NoteViewModel vm = new NoteViewModel();

        [TestMethod]
        public void Test_ReadyOnly_WhenEditModeIsTrue()
        {
            vm.EditMode = true;
            bool expected = false;

            Assert.AreEqual(expected, vm.ReadOnly);
        }
    }
}
