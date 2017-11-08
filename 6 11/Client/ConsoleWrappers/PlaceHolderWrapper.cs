using System;
namespace Client.ConsoleWrappers
{
    public class PlaceHolderWrapper : ConsoleWrappersBase
    {
        private string content;
        public string Content
        {
            get
            {
                return content;
            }
            set
            {
                content = value;
                Render();
            }
        }

        public PlaceHolderWrapper(string content)
        {
            Content = content;
        }

        protected override void RenderActionSection()
        {
        }

        protected override void RenderContent()
        {
            Console.WriteLine(content);
        }
    }
}
