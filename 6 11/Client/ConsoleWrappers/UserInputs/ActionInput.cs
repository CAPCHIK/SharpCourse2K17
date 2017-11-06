using System;
namespace Client.ConsoleWrappers.UserInputs
{
    public class ActionInput : UserInput
    {
        public byte ActionIndex { get; }

        public ActionInput(byte actionIndex)
        {
            ActionIndex = actionIndex;
        }
    }
}
