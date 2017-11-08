using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client.ConsoleWrappers
{
    class OnlineUsersWrapper : ConsoleWrappersBase
    {
        private List<string> userNames;
        protected override void RenderActionSection()
        {
            WriteLine("enter to exit");
        }

        public void SetUsersList(List<string> names)
        {
            userNames = names;
            Render();
        }
        
        protected override void RenderContent()
        {
            if(userNames == null)
            {
                WriteLine("wait for users list");
                return;
            }
            WriteLine($"online {userNames.Count} users");
            WriteLine(string.Join('\n', userNames.Select((S, N) => $"{N}: {S}")));
        }

        public void WaitInput()
        {
            ReadInput();
        }
    }
}
