using System;
namespace Client.ConsoleWrappers.UserInputs
{
    public class StringInput : UserInput
    {
        public string InputString { get; }

        public StringInput(string inputString)
        {
            InputString = inputString;
        }
    }
}
