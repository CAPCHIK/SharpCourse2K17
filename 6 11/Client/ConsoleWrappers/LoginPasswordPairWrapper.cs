using Client.ConsoleWrappers.UserInputs;

namespace Client.ConsoleWrappers
{
    public class LoginPasswordPairWrapper : ConsoleWrappersBase
    {
        private readonly string content;
        private string login = null;
        private string password = null;
        
        public LoginPasswordPairWrapper(string content)
        {
            this.content = content;
            Render();
        }
        protected override void RenderActionSection()
        {
        }

        protected override void RenderContent()
        {
            System.Console.WriteLine(content);
            System.Console.WriteLine();
            System.Console.WriteLine($"login: {login ?? "..."}");
            System.Console.WriteLine($"pass (only digits): {password ?? "..."}");
        }

        public LoginPasswordPair GetPair()
        {
            login = ReadString();
            Render();
            var numPass = ReadNumber();
            password = numPass.ToString();
            Render();
            return new LoginPasswordPair
            {
                Login = login,
                Password = numPass
            };
        }

        private int ReadNumber()
        {

            int number;
            while (!int.TryParse(ReadString(), out number))
            {
                Render();
            }
            return number;
        }
        private string ReadString()
        {
            while (true)
            {
                var input = ReadInput();
                if (input is StringInput str)
                    return str.InputString;
            }
        }

    }
}