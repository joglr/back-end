﻿
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PolloPollo.Repository;
using PolloPollo.Shared;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Http;

namespace PolloPollo.Web.Controllers
{
    [Authorize]
    // [ApiExplorerSettings(IgnoreApi = true)]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private IUserRepository _userRepository;


        public UsersController(IUserRepository repo)
        {
            _userRepository = repo;

        }

        // POST api/users/authenticate
        [AllowAnonymous]
        [ApiConventionMethod(typeof(DefaultApiConventions),
                     nameof(DefaultApiConventions.Post))]
        [HttpPost("authenticate")]
        public async Task<ActionResult<TokenDTO>> Authenticate([FromBody] AuthenticateDTO userParam)
        {
            var (userDTO, token) = await _userRepository.Authenticate(userParam.Email, userParam.Password);

            if (token == null || userDTO == null)
            {
                return BadRequest("Username or password is incorrect");
            }

            return new TokenDTO
            {
                Token = token,
                UserDTO = userDTO
            };
        }

        // GET api/users/42
        [ApiConventionMethod(typeof(DefaultApiConventions),
             nameof(DefaultApiConventions.Get))]
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> Get(int id)
        {
            var user = await _userRepository.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // POST api/users
        [AllowAnonymous]
        [ProducesResponseType(typeof(TokenDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost]
        public async Task<ActionResult<TokenDTO>> Post([FromBody] UserCreateDTO dto)
        {
            if (dto.Role == null || !Enum.IsDefined(typeof(UserRoleEnum), dto.Role))
            {
                return BadRequest("Users must have a assigned a valid role");
            }

            var created = await _userRepository.CreateAsync(dto);

            // Already exists
            if (created == null)
            {
                return Conflict();
            }

            return CreatedAtAction(nameof(Get), new { id = created.UserDTO.UserId }, created);
        }

        // PUT api/users/5
        [HttpPut]
        public async Task<ActionResult> Put([FromBody] UserUpdateDTO dto)
        {
            // Identity check of current user, if emails don't match,
            // it is an unauthorized call
            if (!GetAssociatedUserEmail(User).Equals(dto.Email))
            {
                return Unauthorized();
            }

            var result = await _userRepository.UpdateAsync(dto);

            if (!result)
            {
                return BadRequest();
            }

            return NoContent();
        }
    }
}
