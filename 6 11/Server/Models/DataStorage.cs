using System;
using System.Collections.Generic;
using System.Linq;
using Shared.Models.Users;

namespace Server.Models
{
    public class DataStorage
    {
        public List<User> Users { get; } = new List<User>();

        public void RegisterUser(User user)
        {
            Users.Add(user);
        }

        public User Find(Func<User, bool> predicate)
        {
            return Users.FirstOrDefault(predicate);
        }
     }
}
