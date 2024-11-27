using LidgrenServer.Services;
using LidgrenServer.Models;

namespace LidgrenServer.Controllers
{
    public class UserController
    {
        private readonly UserService _userService;
        public UserController(UserService userService)
        {
            _userService = userService;
        }

        public async Task<UserModel> Login(string username, string password)
        {
            var user = await _userService.GetUserByUsernameAsync(username);
            
            if (user != null && user.VerifyPassword(password))
            {
                Logging.Info(user.Username);
                return user;
            }
            return null;
        }

        public async Task ChangeDisplayName(string username, string newDisplayName)
        {
            var user = await getUserInfoByUserNameAsync(username);
            user.Display_name = newDisplayName;
            await _userService.UpdateUserAsysn(user);
        }

        public async Task<UserModel> getUserInfoByUserNameAsync(string username)
        {
            return await _userService.GetUserByUsernameAsync(username);
        }

        public async Task<bool> SignUp(string username, string password)
        {
            var user = await getUserInfoByUserNameAsync(username);
            if (user != null) 
            { 
                return false;
            }
            var newUser = new UserModel
            {
                Username = username,
                Password = password,
                Coin = 100,
            };
            await _userService.CreateNewUserAsync(newUser);
            return true;
        }

        public async Task<bool> CreateSampleUser()
        {
            try
            {
                
                var newUser = new UserModel
                {
                    Username = "testUser",
                    Password = "password123",
                    Display_name = "Test User",
                    Coin = 100
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
        
        //public async Task SetUserOnlineAsync(UserModel user)
        //{
        //    user.IsOnline = true;
        //    await _userService.UpdateUserAsysn(user);
        //}
    }
}
