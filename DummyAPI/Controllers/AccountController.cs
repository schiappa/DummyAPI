using DummyAPI.Models;
using DummyAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace DummyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : BaseAPIController
    {
        [Route("Login")]
        [HttpPost]
        public ActionResult Login([FromBody] LoginDTO login)
        {
            if (login == null || string.IsNullOrWhiteSpace(login.Username) || string.IsNullOrWhiteSpace(login.Password))
                return BadRequest("Se debe indicar el email y contraseña");
            try
            {
                
                UserDTO user = SessionRepository.GetUserWithCredentials(login.Username, login.Password);
                if (user == null)
                    return BadRequest("Usuario o contraseña incorrectos.");

                string token = SessionRepository.NewSession(user.Id);
                var result = new
                {
                    user.Id,
                    user.Name,
                    user.Lastname,
                    user.Email,
                    user.Username,
                    token,
                };
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Route("GetUsers")]
        [HttpPost]
        public ActionResult GetUsers([FromBody] string search)
        {
            return AuthorizeUser(authorizedUser =>
            {
                try
                {
                    List<UserDTO> result = UsersRepository.GetUsers(search);
                    return Ok(result);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.ToString());
                }
            });
        }

        [Route("Get")]
        [HttpGet]
        public ActionResult Get(int id)
        {
            return AuthorizeUser(authorizedUser =>
            {
                try
                {
                    UserDTO result = UsersRepository.GetById(id);
                    if (result != null)
                        return Ok(result);
                    return BadRequest("Usuario no encontrado");
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.ToString());
                }
            });
        }

        [Route("Create")]
        [HttpPost]
        public ActionResult Create([FromBody] UserDTO user)
        {
            return AuthorizeUser(authorizedUser =>
            {
                try
                {
                    int result = UsersRepository.Create(user, out string errorMsg);
                    if(result > 0)
                        return Ok(result);
                    return BadRequest(errorMsg);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.ToString());
                }
            });
        }

        [Route("Edit")]
        [HttpPut]
        public ActionResult Edit(int id, [FromBody] UserDTO user)
        {
            return AuthorizeUser(authorizedUser =>
            {
                try
                {
                    bool result = UsersRepository.Edit(id, user, out string errorMsg);
                    if (result)
                        return Ok(result);
                    return BadRequest(errorMsg);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.ToString());
                }
            });
        }

        [Route("Delete")]
        [HttpDelete]
        public ActionResult Delete(int id)
        {
            return AuthorizeUser(authorizedUser =>
            {
                try
                {
                    bool result = UsersRepository.Delete(id);
                    if (result)
                        return Ok(result);
                    return BadRequest("Usuario no encontrado");
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.ToString());
                }
            });
        }

        [Route("Logout")]
        [HttpPost]
        public ActionResult Logout()
        {
            try
            {
                string token = GetSessionFromHeader();
                SessionRepository.RemoveSession(token);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest($"Se dio un error al procesar la solicitud: {e.Message}");
            }
        }

        [Route("DummyGet")]
        [HttpGet]
        public ActionResult DummyGet(int id)
        {
            UserDTO result = UsersRepository.GetById(id);
            if (result != null)
                return Ok(result);
            return BadRequest("Usuario no encontrado");
        }
    }
}