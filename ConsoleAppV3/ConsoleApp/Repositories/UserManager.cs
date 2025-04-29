using System;
using System.IO;
using System.Text.Json;
using ConsoleAppV3.Interface;
using ConsoleAppV3.Models;

namespace ConsoleAppV3.Repositories
{
    public class UserManager : IUserManager
    {
        private const string UserFilePath = "user.json";
        private User _currentUser;

        public UserManager()
        {
            _currentUser = LoadUser();
        }

        public void RegisterUser(User user)
        {
            if (_currentUser != null)
            {
                ClearUser();
                throw new InvalidOperationException("User already exists");
            }
            _currentUser = user;
            SaveUser();
        }

        public User GetUser() => _currentUser;

        public void SaveUser()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(UserFilePath, JsonSerializer.Serialize(_currentUser, options));
        }

        public void ClearUser()
        {
            try
            {
                if (File.Exists(UserFilePath))
                    File.Delete(UserFilePath);
            }
            catch { /* Handle exception if needed */ }
            _currentUser = null;
        }

        private User LoadUser()
        {
            if (!File.Exists(UserFilePath)) return null;
            try
            {
                return JsonSerializer.Deserialize<User>(File.ReadAllText(UserFilePath));
            }
            catch
            {
                return null;
            }
        }
    }
}