using DummyAPI.Database;
using DummyAPI.Models;
using System.Collections.Generic;

namespace DummyAPI.Repositories
{
    public static class UsersRepository
    {
        public static List<UserDTO> GetUsers(string search)
        {
            return DBManager.GetUsers(search);
        }

        public static int Create(UserDTO user, out string errorMsg)
        {
            if (user == null)
            {
                errorMsg = "El usuario no puede ser nulo";
                return -1;
            }
            if (string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.Password))
            {
                errorMsg = "El nombre de usuario y la contraseña son campos obligatorios";
                return -1;
            }
            user.PasswordHash = SessionRepository.GeneratePasswordHash(user.Password);
            return DBManager.CreateUser(user, out errorMsg);
        }

        public static UserDTO GetById(int id)
        {
            return DBManager.GetUser(id);
        }

        public static bool Delete(int id)
        {
            return DBManager.DeleteUser(id);
        }

        public static bool Edit(int id, UserDTO user, out string errorMsg)
        {
            if (user == null)
            {
                errorMsg = "El usuario no puede ser nulo";
                return false;
            }
            if (string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.Password))
            {
                errorMsg = "El nombre de usuario y la contraseña son campos obligatorios";
                return false;
            }
            if(!string.IsNullOrWhiteSpace(user.Password))
                user.PasswordHash = user.PasswordHash = SessionRepository.GeneratePasswordHash(user.Password); ;
            return DBManager.Edit(id, user, out errorMsg);
        }
    }
}
