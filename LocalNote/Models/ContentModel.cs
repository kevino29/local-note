using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalNote.Models {
    public class ContentModel {
        public string Rtf { get; set; }
        public string PlainText { get; set; }

        public ContentModel() {
            Rtf = @"{\rtf1\fbidis\ansi\ansicpg1252\deff0\nouicompat\deflang1033{\fonttbl{\f0\fnil Segoe UI;}}{\colortbl ;\red255\green255\blue255;}{\*\generator Riched20 10.0.19041}\viewkind4\uc1 \pard\tx720\cf1\f0\fs27\lang4105\par}";
            PlainText = "";
        }
        public ContentModel(string rtf, string plainText) {
            Rtf = rtf;
            PlainText = plainText;
        }
    }
}
