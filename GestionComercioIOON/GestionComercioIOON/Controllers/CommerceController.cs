using GestionComercioIOON.DTO;
using GestionComercioIOON.IServices;
using GestionComercioIOON.Repositories;
using GestionComercioIOON.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GestionComercioIOON.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommerceController : ControllerBase
    {
        private readonly CommerceRepository _commerceService = new CommerceRepository() ;

        [HttpPost("create")]
        public async Task<IActionResult> CreateCommerceAndOwner([FromBody] CreateCommerceDto dto)
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

            var message =  _commerceService.CreateCommerceAndOwner(dto.CommerceName, dto.Address, dto.Ruc, dto.Username, dto.Password, dto.FullName, dto.Email, dto.Phone, dto.Role);
            return Ok(message);
        }

        [HttpDelete("delete/{userId}")]
        public async Task<IActionResult> DeleteUserAndCommerce(string userId)
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

            var message =  _commerceService.DeleteUserAndCommerce(userId);
            return Ok(message);
        }

        [HttpPost("adduser")]
        public async Task<IActionResult> AddUserToCommerce([FromBody] AddUserDto dto)
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

            var message = _commerceService.AddUserToCommerce(dto.Username, dto.Password, dto.Role, dto.CommerceId);
            return Ok(message);
        }
    }

}
