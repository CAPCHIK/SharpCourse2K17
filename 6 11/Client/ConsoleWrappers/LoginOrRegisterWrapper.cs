using Client.ConsoleWrappers.UserInputs;

namespace Client.ConsoleWrappers
{
    public class LoginOrRegisterWrapper : ConsoleWrappersBase
    {
        protected override void RenderActionSection()
        {
            System.Console.WriteLine("/1 login /2 register");
        }

        protected override void RenderContent()
        {
            System.Console.WriteLine("Hi! enter action for login ot register!");
        }

        public UserInput GetUserChoose()
        {
            var input = ReadInput();
            switch (input)
            {
                case StringInput str:
                    Render();
                    return GetUserChoose();
                case ActionInput action when (action.ActionIndex == 1) :
                    return input;
                case ActionInput action when (action.ActionIndex == 2) :
                    return input;
                default :
                    Render();
                    return GetUserChoose();
            }
        }
    }
}