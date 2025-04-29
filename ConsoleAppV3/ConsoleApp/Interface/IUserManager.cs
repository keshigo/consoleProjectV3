using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConsoleAppV3.Models;

namespace ConsoleAppV3.Interface
{
    public interface IUserManager
    {
        void RegisterUser(User user);
        User GetUser();
        void SaveUser();
        void ClearUser();
    }
}