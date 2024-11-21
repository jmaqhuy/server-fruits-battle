using LidgrenServer.model;
using LidgrenServer.services;
using System.Text.Json;

namespace LidgrenServer.controllers
{
    public class UserController
    {
        private readonly UserService _userService;
        public UserController(UserService userService)
        {
            _userService = userService;
        }

        public async Task<bool> Login(string username, string password)
        {
            var user = await _userService.GetUserByUsernameAsync(username);
            
            if (user != null && user.VerifyPassword(password))
            {
                Logging.Info(user.Username);
                return true;
            }
            return false;
        }

        public async Task<bool> CreateSampleUser()
        {
            try
            {
                var newUser = new UserModel
                {
                    Username = "testUser",
                    Password = "password123",
                    display_name = "Test User",
                    coin = 100
                };
                await _userService.CreateNewUserAsync(newUser);
                return true;
            }
            catch (Exception ex)
            {
                Logging.Error(ex.ToString());
                return false;
            }
        }
    }
}
