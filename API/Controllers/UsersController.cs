using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Collections.Generic;


namespace API.Controllers
{
    public class UserExample : IExamplesProvider<User>
    {
        public User GetExamples()
        {
            return new User
            {
                Id = "8",
                Name = "Новий2 Користувач",
                Email = "new.user@example.com"
            };
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly CreatorUsers _userCreator;

        public UserController(CreatorUsers userCreator)
        {
            _userCreator = userCreator;
        }

        // Отримати список всіх користувачів
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<User>))]
        public ActionResult<IEnumerable<User>> GetUsers()
        {
            var users = _userCreator.TestUsers;
            return Ok(users);
        }

        // Знайти користувача за ідентифікатором
        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(User))]
        [ProducesResponseType(404)]
        public ActionResult<User> GetUserById(string id)
        {
            var user = _userCreator.TestUsers.Find(u => u.Id == id);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        // Знайти користувача за ідентифікатором і видалити його
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public IActionResult DeleteUserById(string id)
        {
            var user = _userCreator.TestUsers.Find(u => u.Id == id);
            if (user == null)
                return NotFound();

            _userCreator.DeleteUserById(id);
            return Ok();
        }

        // Створити нового користувача та отримати його ідентифікатор
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(User))]
        [SwaggerRequestExample(typeof(User), typeof(UserExample))]
        public ActionResult<User> CreateUser([FromBody] User newUser)
        {
            var createdUser = _userCreator.CreateNewUserVoidPost(newUser.Id, newUser.Name, newUser.Email);

            return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser);
        }

        [HttpPut("create/{id}")]
        [ProducesResponseType(200, Type = typeof(User))]
        [ProducesResponseType(404)]
        public IActionResult UpdateUserById(string id, [FromBody] User updatedUser)
        {
            var existingUser = _userCreator.TestUsers.Find(u => u.Id == id);
            if (existingUser == null)
                return NotFound();

            existingUser.Name = updatedUser.Name;
            existingUser.Email = updatedUser.Email;

            return Ok(existingUser);
        }
    }
}


