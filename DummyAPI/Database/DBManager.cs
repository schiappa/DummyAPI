using DummyAPI.Models;
using System.Collections.Generic;
using System.Linq;

namespace DummyAPI.Database
{
    public static class DBManager
    {
        private static List<UserDTO> SystemUsers = new List<UserDTO>()
        {
            new UserDTO()
            {
                Active = true,
                Email = "test@test.com",
                Id = 1,
                Name = "Roberto",
                Lastname = "López",
                Username = "roberto.lopez",
                PasswordHash = "FC82BCFFA537D660C406FE10855887C7" //Test.1
            },
            new UserDTO()
            {
                Active = true,
                Email = "seba@chiappa.com",
                Id = 2,
                Name = "Sebastián",
                Lastname = "Chiappa",
                Username = "seba.chiappa",
                PasswordHash = "185ACEF105ED2537AA290BCB9D4C5AB5" //Seba.1
            },
            new UserDTO()
            {
                Active = true,
                Email = "admin@admin.com",
                Id = 3,
                Name = "Administrador",
                Lastname = "Administrador",
                Username = "admin",
                PasswordHash = "577B8EB711461DA3D18B0DB473FC2658" //Admin.1
            },
        };

        public static UserDTO LoginWithEmail(string email, string passwordHash)
        {
            UserDTO result = SystemUsers.Where(x => x.Active && x.Email.ToLower() == email.ToLower() && x.PasswordHash == passwordHash)
                .Select(x => new UserDTO() { Id = x.Id, Name = x.Name, Lastname = x.Lastname, Active = x.Active, Email = x.Email, Username = x.Username })
                .FirstOrDefault();
            if (result != null)
                result.PasswordHash = "";
            return result;
        }

        public static UserDTO LoginWithUsername(string username, string passwordHash)
        {
            UserDTO result = SystemUsers.Where(x => x.Active && x.Username.ToLower() == username.ToLower() && x.PasswordHash == passwordHash)
                .Select(x=>new UserDTO() {Id = x.Id, Name = x.Name, Lastname = x.Lastname, Active = x.Active, Email = x.Email, Username = x.Username })
                .FirstOrDefault();
            if(result != null)
                result.PasswordHash = "";
            return result;
        }

        public static UserDTO GetUser(int id)
        {
            UserDTO result = SystemUsers.Where(x => x.Active && x.Id == id)
                .Select(x => new UserDTO() { Id = x.Id, Name = x.Name, Lastname = x.Lastname, Active = x.Active, Email = x.Email, Username = x.Username })
                .FirstOrDefault();
            if (result != null)
                result.PasswordHash = "";
            return result;
        }

        public static int CreateUser(UserDTO user, out string errorMsg)
        {
            errorMsg = "";
            if (SystemUsers.Any(x => x.Username.ToLower() == user.Username.ToLower()))
            {
                errorMsg = $"El nombre de usuario \"{user.Username}\" ya se encuentra utilizado";
                return -1;
            }
            int id = SystemUsers.Max(x => x.Id) + 1;
            user.Id = id;
            SystemUsers.Add(user);
            return id;
        }

        public static bool DeleteUser(int id)
        {
            bool result = false;
            if(SystemUsers.Any(x=>x.Active && x.Id == id))
            {
                UserDTO user = SystemUsers.FirstOrDefault(x => x.Id == id);
                user.Active = false;
                SystemUsers.RemoveAll(x => x.Id == id);
                SystemUsers.Add(user);
                result = true;
            }
            return result;
        }

        public static bool Edit(int id, UserDTO newUser, out string errorMsg)
        {
            bool result = false;
            errorMsg = "";
            if (SystemUsers.Any(x => x.Active && x.Id == id))
            {
                if(SystemUsers.Any(x=>x.Id != id && x.Username.ToLower() == newUser.Username.ToLower()))
                {
                    errorMsg = $"El nombre de usuario \"{newUser.Username}\" no se encuentra disponible";
                    return result;
                }
                UserDTO user = SystemUsers.FirstOrDefault(x => x.Id == id);
                user.Active = newUser.Active;
                user.Name = newUser.Name;
                user.Lastname = newUser.Lastname;
                user.Email = newUser.Email;
                user.Username = newUser.Username;
                if (!string.IsNullOrWhiteSpace(newUser.PasswordHash)) 
                    user.PasswordHash = newUser.PasswordHash;
                SystemUsers.RemoveAll(x => x.Id == id);
                SystemUsers.Add(user);
                result = true;
            }
            return result;
        }

        public static List<UserDTO> GetUsers(string searchStr = null)
        {
            return !string.IsNullOrWhiteSpace(searchStr) ? SystemUsers.Where(x => x.Active && 
                (
                    x.Username.ToLower().Contains(searchStr.ToLower()) ||
                    x.Name.ToLower().Contains(searchStr.ToLower()) ||
                    x.Lastname.ToLower().Contains(searchStr.ToLower()) ||
                    x.Email.ToLower().Contains(searchStr.ToLower())
                )).Select(x => new UserDTO() { Id = x.Id, Name = x.Name, Lastname = x.Lastname, Active = x.Active, Email = x.Email, Username = x.Username })
            .ToList() 
            : 
            SystemUsers.Where(x => x.Active)
            .Select(x => new UserDTO() { Id = x.Id, Name = x.Name, Lastname = x.Lastname, Active = x.Active, Email = x.Email, Username = x.Username })
            .ToList();
        }
    }
}
