using System;
using System.Collections.Generic;
using System.Linq;
using Shared.Models.Users;

namespace Server.Models
{
    //Класс хранилище данных о пользователе
    //Тут мы не храним данные в файле, 
    //А просто в оперативе
    //При желании, не долго переписать на файл
    //Но на данный момент база пользователей будет существовать
    //Только пока сервер работает
    public class DataStorage
    {
        //Список пользователей
        public List<User> Users { get; } = new List<User>();
        //Регистрация пользователя в системе
        public void RegisterUser(User user)
        {
            //Просто добавляем пользователя в список
            Users.Add(user);
        }
        //Поиск пользователя по какому-то признаку
        public User Find(Func<User, bool> predicate)
        {   
            //Находим этого пользователя по признаку
            //Или возвращаем null, если такового нет
            return Users.FirstOrDefault(predicate);
        }
     }
}
