using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DummyAPI.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace DummyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseAPIController : ControllerBase
    {
        public static string AuthorizationHeader = "Authorization";
        public static string AuthorizationTokenPrefix = "Bearer ";
        private static readonly string SecretToken = "oITQ86J+s7vz7thJArHfTY0tDys28Z8lUNXtRchELkI=";

        [ApiExplorerSettings(IgnoreApi = true)]
        public string GetSessionFromHeader()
        {
            try
            {
                Request.Headers.TryGetValue(AuthorizationHeader, out StringValues value);
                string header = value.First();
                string token = header.Replace("Bearer ", "");// header.Substring(AuthorizationTokenPrefix.Count());
                return token;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Authorization token not found.", ex);
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public FileStreamResult AuthorizeUserFileStream(Func<int, FileStreamResult> authorizedUserCallback)
        {
            int userId;
            try
            {
                userId = GetUserFromHeaderSession();
                if (userId == 0)
                    return null;
            }
            catch (Exception)
            {
                return null;
            }
            return authorizedUserCallback(userId);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult AuthorizeUser(Func<int, ActionResult> authorizedUserCallback)
        {
            int userId;
            try
            {
                userId = GetUserFromHeaderSession();
                if (userId == 0)
                    return Unauthorized("No existe la sesión.");
            }
            catch (Exception)
            {
                return Unauthorized();
            }
            return authorizedUserCallback(userId);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult AuthorizeToken(bool skipLicenseCheck, bool skipUser, Func<int, ActionResult> authorizedUserCallback)
        {
            int userId;
            try
            {
                if (!skipUser)
                {
                    userId = GetUserFromHeaderSession();
                    if (userId == 0)
                        return Unauthorized("No existe la sesión.");
                }
                else
                {
                    if (!CheckSecretTokenFromHeader())
                        return Unauthorized("Invalid token");
                    userId = -1;
                }
            }
            catch (Exception)
            {
                return Unauthorized();
            }
            return authorizedUserCallback(userId);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        private int GetUserFromHeaderSession()
        {
            string token = GetSessionFromHeader();
            return SessionRepository.GetUserId(token);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        private bool CheckSecretTokenFromHeader()
        {
            string token = GetSessionFromHeader();
            return token == SecretToken;
        }
    }
}