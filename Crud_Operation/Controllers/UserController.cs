using Crud_Operation.Model;
using Crud_Operation.Services;
using Crud_Operation.Services.Interface;
using Crud_Operation.Services.Token;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Crud_Operation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userservice;
        private readonly ITokenService _tokenService;

        public UserController(IConfiguration configuration, IUserService userService, ITokenService tokenService)
        {
            _userservice = userService;
            _tokenService = tokenService;
        }

        [Route("GetAll")]
        [HttpGet]
        public async Task<ActionResult<ResponseData>> GetAll()
        {
            try
            {
                var users = await _userservice.GetAll();
                var response = new ResponseData
                {
                    success = true,
                    message = "Get All Users succesfully",
                    code = 200,
                    data = users
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new ResponseData
                {
                    success = false,
                    message = ex.Message,
                    code = 500
                };
                return StatusCode(500, response);
            }
        }

        [Route("GetAllPaged")]
        [HttpGet]
        public async Task<ActionResult<ResponseData>> GetAllPaged(int pageNumber, int pageSize)
        {
            try
            {
                var users = await _userservice.GetAllPaged(pageNumber, pageSize);
                var totalUsers = await _userservice.GetTotalCount();

                var response = new ResponseData 
                {
                    success = true,
                    message = "Get All Users successfully",
                    code = 200,
                    data = new
                    {
                        users,
                        totalUsers
                    }
                };
                
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new ResponseData
                {
                    success = false,
                    message = ex.Message,
                    code = 500
                };
                return StatusCode(500, response);
            }
        }


        [Route("GetById")]
        [HttpGet]
        public async Task<ActionResult<ResponseData>> GetById()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");


            var responseData = new ResponseData();
            try
            {
                var userIdString = _tokenService.GetUserIdFromToken(token);
                if (userIdString == null || !int.TryParse(userIdString, out int userId))
                {
                    return Unauthorized(new ResponseData
                    {
                        success = false,
                        message = "Unauthorized: Invalid Token",
                        code = 401
                    });
                }
                var user = await _userservice.GetById(userId);
                if (user == null)
                {
                    responseData.success = false;
                    responseData.message = "User Not Found!";
                    responseData.code = 404;
                    return NotFound(responseData); // Return NotFound response
                }

                responseData.success = true;
                responseData.message = "Get User succesfully";
                responseData.data = user;
                responseData.code = 200;
                return Ok(responseData);
            }
            catch (Exception ex)
            {
                responseData.success = false;
                responseData.message = ex.Message;
                responseData.code = 500;
                return StatusCode(500, responseData); // Return StatusCode for exceptions
            }
        }


        [Route("Update")]
        [HttpPut]
        public async Task<ActionResult<ResponseData>> Update([FromBody] User updateModel)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            try
            {
                var userIdString = _tokenService.GetUserIdFromToken(token);
                if (userIdString == null || !int.TryParse(userIdString, out int userId))
                {
                    return Unauthorized(new ResponseData
                    {
                        success = false,
                        message = "Unauthorized: Invalid Token",
                        code = 401
                    });
                }

                updateModel.Id = userId;
                var updatedUser = await _userservice.Update(updateModel);
                if (updatedUser != null)
                {
                    return Ok(new ResponseData
                    {
                        success = true,
                        message = "User updated successfully",
                        data = updatedUser,
                        code = 200
                    });
                }
                else
                {
                    return BadRequest(new ResponseData
                    {
                        success = false,
                        message = "User not updated successfully!",
                        code = 400
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseData
                {
                    success = false,
                    message = ex.Message,
                    code = 500
                });
            }
        }

        [Route("Delete")]
        [HttpDelete]
        public async Task<ActionResult<ResponseData>> Delete()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            try
            {
                var userIdString = _tokenService.GetUserIdFromToken(token);
                if (userIdString == null || !int.TryParse(userIdString, out int userId))
                {
                    return Unauthorized(new ResponseData
                    {
                        success = false,
                        message = "Unauthorized: Invalid Token",
                        code = 401
                    });
                }

                var isUserDeleted = await _userservice.Delete(userId);
                return Ok(new ResponseData
                {
                    success = isUserDeleted,
                    message = isUserDeleted ? "User Deleted Successfully" : "User not found",
                    data = isUserDeleted,
                    code = isUserDeleted ? 200 : 404
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseData
                {
                    success = false,
                    message = $"An error occurred: {ex.Message}",
                    code = 500
                });
            }
        }
    }
}