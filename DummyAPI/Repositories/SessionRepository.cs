using DummyAPI.Database;
using DummyAPI.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace DummyAPI.Repositories
{
    public class SessionRepository
    {
        private static ConcurrentDictionary<int, List<string>> _currentSessions = new ConcurrentDictionary<int, List<string>>();

        private static string GenerateToken()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        }

        public static string NewSession(int userId)
        {
            KeyValuePair<int, List<string>> currentSession = _currentSessions.FirstOrDefault(s => s.Key == userId);
            string token = GenerateToken();
            if (currentSession.Key == 0)
                _currentSessions.TryAdd(userId, new List<string> { token });
            else
                currentSession.Value.Add(token);
            return token;
        }

        internal static UserDTO GetUserWithCredentials(string email, string password)
        {
            string passwordHash = GeneratePasswordHash(password);
            EmailAddressAttribute validator = new EmailAddressAttribute();
            if (validator.IsValid(email)) //Se verifica si email es un email o si email es en realidad un username
                return DBManager.LoginWithEmail(email, passwordHash);
            return DBManager.LoginWithUsername(email,passwordHash);
        }

        public static int GetUserId(string token)
        {
            return _currentSessions.FirstOrDefault(s => s.Value.Any(t => t.Equals(token))).Key;
        }

        public static void RemoveSession(string token)
        {
            int userId = GetUserId(token);
            if (userId == 0)
                return;
            List<string> tokens = _currentSessions.FirstOrDefault(s => s.Value.Any(t => t.Equals(token))).Value;
            tokens.Remove(token);
            _currentSessions.TryUpdate(userId, tokens, tokens);
        }

        internal static string GeneratePasswordHash(string password)
        {
            using (MD5CryptoServiceProvider sha = new MD5CryptoServiceProvider())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                return string.Concat(sha.ComputeHash(passwordBytes).Select(x => x.ToString("X2")));
            }
        }

        internal UserDTO GetUser(int authorizedUser)
        {
            return DBManager.GetUser(authorizedUser);
        }
    }
}
