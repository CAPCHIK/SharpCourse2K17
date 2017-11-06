using System;
using System.Collections.Generic;
using System.Linq;
using Client.ConsoleWrappers.UserInputs;

namespace Client.ConsoleWrappers
{
    public class SimpleWrapper : ConsoleWrappersBase
    {
        private List<string> rows = new List<string>();

        public SimpleWrapper() : base()
        {
            Render();
        }

        protected override void RenderContent()
        {
            foreach (var row in rows)
                Console.WriteLine(row);
        }

        public string ReadLine()
        {
            var readedString = ReadInput();
            switch (readedString)
            {
                case StringInput str:
                    AddString(str.InputString);
                    return str.InputString;
                case ActionInput action when (action.ActionIndex == 1):
                    foreach (var i in Enumerable.Range(1, 3))
                        AddString(new string(',', i));
                    break;
                case ActionInput action when (action.ActionIndex == 2):
                    break;
            }
            return "";
        }
        public void AddString(string content)
        {
            rows.Add(content);
            Render();
        }

        protected override void RenderActionSection()
        {
            Console.WriteLine("/1: write 3 lines");
        }
    }
}