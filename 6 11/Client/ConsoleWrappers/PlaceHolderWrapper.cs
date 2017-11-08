using System;
namespace Client.ConsoleWrappers
{
    //Форма для отображения простого текста и игнорирования пользовательского ввода
    //Аналог формы загрузки
    public class PlaceHolderWrapper : ConsoleWrappersBase
    {
        //Контент, который будет показан пользователю
        private string content;
        //Публичноен свойство Content
        //Предоставляет доступ к приватному полю
        public string Content
        {
            //При чтении поля просто возвращает значение поля
            get
            {
                return content;
            }
            //При обновлении устанавливает поле в переданное значение
            set
            {
                content = value;
                //И рендерит новое содержимое
                Render();
            }
        }
        //Кнструктор формы принимает текст для отображения
        public PlaceHolderWrapper(string content)
        {
            Content = content;
        }
        //Секция действий в данной форме отсутствует, так что для секции
        //Действий ничего не делаем
        protected override void RenderActionSection()
        {
        }
        //В секции отрисовки контента выводим строку, сообщение заглушки
        protected override void RenderContent()
        {
            Console.WriteLine(content);
        }
    }
}
