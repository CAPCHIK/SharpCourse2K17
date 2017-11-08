using System;
using System.Collections.Generic;
using Client.ConsoleWrappers.UserInputs;

namespace Client.ConsoleWrappers
{
    //Класас, определяющий базовую работу нашей оболочки над консолью
    //Его внутренняя кухня Если и будет рассмотрена - то в самом конце
    public abstract class ConsoleWrappersBase : IDisposable
    {
        private object sync = new object();
        protected abstract void RenderContent();
        protected abstract void RenderActionSection();
        private bool rendering = true;

        protected ConsoleWrappersBase()
        {
            Console.CursorVisible = false;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;
            Render();
        }

        public void StartRendering()
        {
            rendering = true;
            Render();
        }
        public void StopRendering()
        {
            rendering = false;
        }

        //Буфер символов, которые вводятся пользователем
        private List<char> buffer = new List<char>();
        private int cursorPosition;
        protected void Render()
        {
            if (!rendering)
                return;
            lock (sync)
            {
                Console.Clear();
                RenderContent();
                Console.WriteLine();
                RenderActionSection();
                
                Console.Write(">>");
                for (int i = 0; i < buffer.Count; i++)
                {
                    if(i == cursorPosition)
                        RenderSymbol(buffer[i]);
                    else
                        Console.Write(buffer[i]);
                }
                if(cursorPosition == buffer.Count)
                    RenderSymbol(' ');
            }
        }
        private void RenderSymbol(char symbol)
        {
            var backColor = Console.BackgroundColor;
            var textColor = Console.ForegroundColor;
            Console.BackgroundColor = textColor;
            Console.ForegroundColor = backColor;
            Console.Write(symbol);
            Console.BackgroundColor = backColor;
            Console.ForegroundColor = textColor;
        }
        protected void WriteLine(string line) => WriteLine(line, ConsoleColor.Gray);
        protected void WriteLine(string line, ConsoleColor color)
        {
            var textColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(line);
            Console.ForegroundColor = textColor;
        }
        protected virtual UserInput ReadInput()
        {
            ConsoleKeyInfo currentKey;
            while ((currentKey = Console.ReadKey()).Key != ConsoleKey.Enter)
            {
                if (char.IsDigit(currentKey.KeyChar)
                    && buffer.Count == 1 
                    && buffer[0] == '/')

                {
                    var actionIndex = byte.Parse(currentKey.KeyChar.ToString());
                    buffer.Clear();
                    cursorPosition = 0;
                    Render();
                    return new ActionInput(actionIndex);
                }
                else
                switch(currentKey.Key)
                {
                    case ConsoleKey.Backspace:
                        if(buffer.Count > 0)
                            buffer.RemoveAt(--cursorPosition);
                        break;
                    case ConsoleKey.Delete: 
                        if(cursorPosition < buffer.Count)
                            buffer.RemoveAt(cursorPosition);
                        break;
                    case ConsoleKey.RightArrow:
                        if(cursorPosition < buffer.Count)
                            cursorPosition++;
                        break;
                    case ConsoleKey.LeftArrow:
                        if(cursorPosition > 0)
                            cursorPosition--;
                        break;
                    default:
                        buffer.Add(currentKey.KeyChar);
                        cursorPosition++;
                        break;
                }
                Render();
            }
            Console.WriteLine();
            var result = new string(buffer.ToArray());
            buffer.Clear();
            cursorPosition = 0;
            return new StringInput(result);
        }

        public void Dispose()
        {
            Console.CursorVisible = true;
            Console.ResetColor();
            Console.Clear();
            Render();
            Console.WriteLine();
        }
    }
}