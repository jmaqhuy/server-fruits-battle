using LidgrenServer.model;
using LidgrenServer.services;

namespace LidgrenServer.controllers
{
    public class UserController
    {
        private readonly UserService _userService;
        public UserController(UserService userService)
        {
            _userService = userService;
        }

        //public async Task<UserModel> Login(int id)
        //{
            
        //}
    }
}
