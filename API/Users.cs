using System;
using System.Collections.Generic;
using System.Linq;

namespace API
{
    public class User
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class DuplicateUserException : Exception
    {
        public DuplicateUserException(string message) : base(message)
        {
        }
    }

    public class CreatorUsers
    {
        private List<User> _testUsers;

        public List<User> TestUsers => _testUsers;

        public CreatorUsers()
        {
            _testUsers = new List<User>
            {
                new User { Id = "1", Name = "John Doe", Email = "john.doe@example.com" },
                new User { Id = "2", Name = "Jane Doe", Email = "jane.doe@example.com" },
                new User { Id = "3", Name = "Alice Smith", Email = "alice.smith@example.com" },
                new User { Id = "4", Name = "Bob Johnson", Email = "bob.johnson@example.com" },
                new User { Id = "5", Name = "Eva Davis", Email = "eva.davis@example.com" }
            };
        }

        public User CreateNewUserVoidPost(string id, string name, string email)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            if (_testUsers.Any(u => u.Id == id))
            {
                throw new DuplicateUserException("User with the same Id already exists.");
            }

            var newUser = new User { Id = id, Name = name, Email = email };

            _testUsers.Add(newUser);

            return newUser;
        }

        public void DeleteUserById(string id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            var userToRemove = _testUsers.Find(u => u.Id == id);
            if (userToRemove == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            _testUsers.Remove(userToRemove);
        }
    }
}