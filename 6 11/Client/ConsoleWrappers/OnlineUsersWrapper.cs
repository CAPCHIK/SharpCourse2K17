using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client.ConsoleWrappers
{
    //Форма для отображения пользователей онлайн
    class OnlineUsersWrapper : ConsoleWrappersBase
    {
        //Поле храняещее список имен
        private List<string> userNames;
        //Для выхода из формы пользователь должен сделать хоть что-то
        //Просто просим его нажать enter
        protected override void RenderActionSection()
        {
            WriteLine("enter to exit");
        }
        //Установка списка пользователей
        public void SetUsersList(List<string> names)
        {
            //сохраняем переданный список имен
            userNames = names;
            //Рендерим этот список
            Render();
        }
        //Рендеринг контента, самого списка
        protected override void RenderContent()
        {
            //Если список имен еще не получен, списка еще просто нет
            if(userNames == null)
            {
                //Показываем сообщение, что ждем пользователей
                WriteLine("wait for users list");
                //И заканчиваем отрисовку
                return;
            }
            //Если пользователи таки переданны, выводим сначала количество пользователей онлайн
            WriteLine($"online {userNames.Count} users");
            //И выводим столбец имен
            //userNames.Select((S, N) => $"{N}: {S}"))
            //Тут S это имя пользователя
            //А N это порядковый номер этого имени в списке имен
            //То есть преобразовываем каждое имя в НОМЕР ИМЕНИ : ИМЯ
            //И при помощи string.join разделяем все строки символом переноса строки
            WriteLine(string.Join('\n', userNames.Select((S, N) => $"{N}: {S}")));
        }
        //Метод ожидания пользовательского ввода, для окончания работы формы
        public void WaitInput()
        {
            //Просто читаем хоть какой ввод, форма предназначена только для отображения
            //Имен, а не для действий
            ReadInput();
        }
    }
}
