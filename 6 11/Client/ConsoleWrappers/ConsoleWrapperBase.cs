using System;
using System.Collections.Generic;
using Client.ConsoleWrappers.UserInputs;

namespace Client.ConsoleWrappers
{
    //Класас, определяющий базовую работу нашей оболочки над консолью
    public abstract class ConsoleWrappersBase : IDisposable
    {
        //Объект для синхронизации
        //Используется для блокирования отрисовки, чтобы
        //Используя форму из нескольких потоков отрисовка не съезжала
        //А всегда работала последовательно, раз за разом
        private object sync = new object();
        //Абстрактный метод отрисовки контента, определяется конкретными классами
        protected abstract void RenderContent();
        //Абстрактный метод отрисовки секции действий, определяется конкретными классами
        protected abstract void RenderActionSection();
        //Поле, показывающее, необходима для отрисовка. По умолчанию она нужна
        private bool rendering = true;
        //Конструктор
        protected ConsoleWrappersBase()
        {
            //Убираем обычный курсор консоли
            Console.CursorVisible = false;
            //Задник будет чисто черным
            Console.BackgroundColor = ConsoleColor.Black;
            //А цвет текста - серый
            Console.ForegroundColor = ConsoleColor.Gray;
            //Рисуем содержимое
            Render();
        }
        //Метод предназначен для возобновления рендеринга
        public void StartRendering()
        {
            //Переводет поле для рендеринга в true
            rendering = true;
            //Рендерит содержимое
            Render();
        }
        //Остановка рендеринга, опускаем флаг
        public void StopRendering()
        {
            rendering = false;
        }

        //Буфер символов, которые вводятся пользователем
        private List<char> buffer = new List<char>();
        //Позиция нашего импровизированного курсора в строке
        private int cursorPosition;
        //Цикл отрисовки
        protected void Render()
        {
            //Если рендерить не надо - ничего не делаем
            if (!rendering)
                return;
            //Сами операции отрисовки будути в синхронизированной секции
            //Вдруг как-то отрисовки вызовутся одновременно
            //Например при получении множества сообщений из сети, во время ввода текста
            //Пользователем
            lock (sync)
            {
                //Чистим содержимое консоли
                Console.Clear();
                //Рисуем контент формы
                RenderContent();
                //Отступ
                Console.WriteLine();
                //Рисуем содержимое секции действий
                RenderActionSection();
                //Указатель на поле для ввода пользовательских данных
                Console.Write(">>");
                //Пробегаем по символам в буффере, уже введенном пользователем
                for (int i = 0; i < buffer.Count; i++)
                {
                    //Если текущий индекс это позиция курсора
                    if(i == cursorPosition)
                        //Рисуем этот символ как курсор
                        RenderSymbol(buffer[i]);
                    else
                        //Иначе просто пишем символ, как обычный символ
                        Console.Write(buffer[i]);
                }
                //Если курсор находится в конце строки
                if(cursorPosition == buffer.Count)
                    //Дорисовываем пробел как курсор, чтобы видеть сам курсор
                    RenderSymbol(' ');
            }
        }
        //Отрисовка курсора
        private void RenderSymbol(char symbol)
        {
            //Предыдущий цвет задника
            var backColor = Console.BackgroundColor;
            //Предыдущий цвет текста
            var textColor = Console.ForegroundColor;
            //Цвет задника меняем на цвет текста
            Console.BackgroundColor = textColor;
            //Цвет текста гна цвет задника
            Console.ForegroundColor = backColor;
            //Пишем символ инвертированными цветами
            Console.Write(symbol);
            //Возвращаем цвет задника на место
            Console.BackgroundColor = backColor;            
            //Возвращаем цвет текста на место
            Console.ForegroundColor = textColor;
        }
        //Метод написания строки, пишет текст серым цветом
        protected void WriteLine(string line) => WriteLine(line, ConsoleColor.Gray);
        //Написание текста конкретным цветом
        protected void WriteLine(string line, ConsoleColor color)
        {
            //Сохраняем старый цвет текста
            var textColor = Console.ForegroundColor;
            //Меняем цвет текста на переданный
            Console.ForegroundColor = color;
            //Пишем текс переданным цветом
            Console.WriteLine(line);
            //Меняем цвет текста на исходный
            Console.ForegroundColor = textColor;
        }
        //Чтение пользовательского ввода
        //Возвращает либо StringInput либо ActionInput
        protected virtual UserInput ReadInput()
        {
            //Переменная для хранения текущей нажатой клавиши
            ConsoleKeyInfo currentKey;
            //Сохраняем считанную клавишу в объявленную выше переменную
            //И проверяем, что это клавиша не Enter
            while ((currentKey = Console.ReadKey()).Key != ConsoleKey.Enter)
            {
                //Если мы ввели цифру
                if (char.IsDigit(currentKey.KeyChar)
                    && buffer.Count == 1 //И у нас до этого был введен только 1 символ
                    && buffer[0] == '/')//И этот символ был /
                {
                    //Получаем индекс действия. Байт испольщуем только по тому что
                    //У нас максимум 10 действий, а байт занимает меньше всего места
                    //В памяти
                    var actionIndex = byte.Parse(currentKey.KeyChar.ToString());
                    //Отчищаем буффер ввода
                    buffer.Clear();
                    //Обнуляем позицию курсора
                    cursorPosition = 0;
                    //Рендерим новое сд=одержимое
                    Render();
                    //Возврощаем ActionInput и индекс в нем
                    return new ActionInput(actionIndex);
                }
                //Пользователь ввел не действие
                else
                //Что же он ввел?
                switch(currentKey.Key)
                {
                    //Если символ удаления (стрелочка налево такая, длинная)
                    case ConsoleKey.Backspace:
                        //Если в буффере есть какие-то символы
                        if(buffer.Count > 0)
                            //Удаляем символ из буффера на позиции курсора, меньшей на 1
                            //При этом сразу же сдвигаем курсор на 1 налево
                            buffer.RemoveAt(--cursorPosition);
                        break;
                    //Если это кнопка delete
                    case ConsoleKey.Delete: 
                        //Если справа от курсора есть символы
                        if(cursorPosition < buffer.Count)
                            //Удаляем символ на месте курсора
                            buffer.RemoveAt(cursorPosition);
                        break;
                    //Если это стрелка вправо
                    case ConsoleKey.RightArrow:
                        //Если справа от курсора есть символы
                        if(cursorPosition < buffer.Count)
                            //двигаем курсор вправо
                            cursorPosition++;
                        break;
                    //Если это стрелка влево
                    case ConsoleKey.LeftArrow:
                        //И курсор не на нуле
                        if(cursorPosition > 0)
                            //Двигаем курсор влево
                            cursorPosition--;
                        break;
                    //Если это не какая-то специальная клавиша, обработанная нами
                    default:
                        //Скорее всего это символ, и мы добавляем его в буффер
                        buffer.Add(currentKey.KeyChar);
                        //Сдвигая курсор направо
                        cursorPosition++;
                        break;
                }
                //При нажатии на каждую клавишу надо рендерить содержимое
                //Так у нас будет подвижный курсор, и удаление символов
                Render();
            }
            //Пишем отступ для красоты чисто
            Console.WriteLine();
            //Собираем строку из введенного буффера
            var result = new string(buffer.ToArray());
            //Отчищаем буффер
            buffer.Clear();
            //Обнуляем курсор
            cursorPosition = 0;
            //Возвращаем StringInput
            return new StringInput(result);
        }
        //Метод для завершения работы формы
        public void Dispose()
        {
            //Возвращаем видимость обычному курсору
            Console.CursorVisible = true;
            //Возвращаем деволтные цвета
            Console.ResetColor();
            //Чистим консоль
            Console.Clear();
            //Рендерим последнее состояние
            Render();
            //Переносим на сл строку
            Console.WriteLine();
        }
    }
}