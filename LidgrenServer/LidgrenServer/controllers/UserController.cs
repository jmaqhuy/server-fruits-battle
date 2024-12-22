using LidgrenServer.Services;
using LidgrenServer.Models;
using System.Text;
using LidgrenServer.controllers;

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

        public async Task<int> GetUserIdByUsernameAsync(string username)
        {
            var user = await _userService.GetUserByUsernameAsync(username);
            return user.Id;
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

        public async Task<UserModel> SignUp(string username, string password, string email)
        {
            var user = await getUserInfoByUserNameAsync(username);
            if (user != null) 
            { 
                return null;
            }
            var newUser = new UserModel
            {
                Username = username,
                Email = email,
                Password = password,
                Coin = 100,
            };
            await _userService.CreateNewUserAsync(newUser);
            
            return newUser;
        }

        public async Task VerifyUserEmail(string username)
        {
            var user = await getUserInfoByUserNameAsync(username);
            user.isVerify = true;
            await _userService.UpdateUserAsysn(user);
        }

        public async Task<bool> isVerifyUserEmail(string username)
        {
            var user = await getUserInfoByUserNameAsync(username);
            return user.isVerify;
        }


        public async Task<bool> ResetPasswordAsync(string username, string email, Random random)
        {
            var user = await _userService.GetUserByUsernameEmailAsync(username,email);
            if (user != null)
            {
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                StringBuilder result = new StringBuilder(8);
                for (int i = 0; i < 8; i++) 
                { 
                    result.Append(chars[random.Next(chars.Length)]);
                }

                user.Password = user.HashPassword(result.ToString());
                Logging.Debug("New Password: " + result.ToString());
                await _userService.UpdateUserAsysn(user);
                var sendmail = new EmailService();
                await sendmail.SendMailResetPassword(user.Username, user.Email, result.ToString());
                return true;
                
            } else
            {
                return false;
            }
        }
        public async Task<UserModel> Changepassword(string username, string newPass)
        {
            var user = await getUserInfoByUserNameAsync(username);
            if (user != null)
            {
                user.Password = user.HashPassword(newPass);
            }
           
            return user;
        }


        //public async Task SetUserOnlineAsync(UserModel user)
        //{
        //    user.IsOnline = true;
        //    await _userService.UpdateUserAsysn(user);
        //}
    }
}
