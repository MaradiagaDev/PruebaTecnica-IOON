namespace GestionComercioIOON.Controllers
{
    using GestionComercioIOON.DTO;
    using GestionComercioIOON.Model;
    using GestionComercioIOON.Repositories;
    using GestionComercioIOON.Services;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;

    namespace YourNamespace.Controllers
    {
        [Route("api/[controller]")]
        [ApiController]
        public class UserController : ControllerBase
        {
            private readonly UserRepository _userService = new UserRepository();

            // Endpoint para hacer login
            [HttpPost("login")]
            public IActionResult Login([FromBody] LoginRequest loginRequest)
            {
                try
                {
                    var user = _userService.Login(loginRequest.Username, loginRequest.Password);
                    if (user == null)
                        return Unauthorized("Credenciales incorrectas");

                    string token = _userService.AuthenticateAsync(user);

                    return Ok(new { user = user, token = token });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, "Error interno del servidor: " + ex.Message);
                }
            }

            // Endpoint para crear o actualizar un usuario
            [HttpPost("updateCreate")]
            public IActionResult UpdateCreateUser([FromBody] User user)
            {
                try
                {
                    var identity = HttpContext.User.Identity as ClaimsIdentity;
                    bool token = Jwt.ValidateToken(identity);

                    if (!token)
                    {
                        return BadRequest(new
                        {
                            success = false,
                            message = "Invalid token.",
                            result = string.Empty
                        });
                    }

                    var result = _userService.UpdateCreateObject(user);
                    return Ok(result);  
                }
                catch (Exception ex)
                {
                    return StatusCode(500, "Error interno del servidor: " + ex.Message);
                }
            }

            [HttpGet]
            public IActionResult GetAllUsers([FromQuery] int offSet, [FromQuery] int pageSize)
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                bool token = Jwt.ValidateToken(identity);

                if (!token)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Invalid token.",
                        result = string.Empty
                    });
                }

                try
                {
                    var users = _userService.GetAllObjects(offSet, pageSize);
                    return Ok(users);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, "Error interno del servidor: " + ex.Message);
                }
            }
        }
    }

}
