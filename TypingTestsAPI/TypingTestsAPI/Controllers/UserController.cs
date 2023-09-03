using TypingTestsAPI.Models;
using TypingTestsAPI.Services;
using Microsoft.AspNetCore.Mvc;
using TypingTestsAPI.Helpers;
using Microsoft.Extensions.Configuration;
using System.Net;
using MongoDB.Bson;
using Newtonsoft.Json.Linq;
using MongoDB.Bson.IO;
using Microsoft.AspNetCore.Cors;
using System;

namespace TypingTestsAPI.Controllers
{
    /* [Route("/user/[controller]")]
     [ApiController]*/
    public class UserController
    {
        private readonly UserService _userService;
        private readonly SessionManager _sessionManager;
        public UserController(UserService userService, SessionManager sessionManager)
        {
            _userService = userService;
            _sessionManager = sessionManager;
        }

        [HttpPost]
        [Route("user/signup")]
       //[ValidateAntiForgeryToken]
        public IActionResult Signup([FromBody] User user)
        {
            Console.WriteLine(user.Username);
            
            try
            {
                if (_userService.CreateUser(user))
                {
                    _sessionManager.CreateUserSession(user);
                    return new StatusCodeResult(200);
                }
                else
                    return new StatusCodeResult(401);
            }
            catch (Exception)
            {
                return new StatusCodeResult(500);
            }
        }
        [HttpPost]
        [Route("user/login")]
        //[ValidateAntiForgeryToken]
        public IActionResult Login([FromBody] User user)
          
        {
            try
            {
                Console.WriteLine("here");
                if (_userService.LoginUser(user))
                {
                    _sessionManager.CreateUserSession(user);
                    return new StatusCodeResult(200);
                }
                else return new StatusCodeResult(401);

            }
            catch (Exception)
            {
                return new StatusCodeResult(500);
            }
        }

        [HttpPost]
        [Route("user/logout")]
        public IActionResult Logout()
        {
            try
            {
                _sessionManager.removeSession();
                return new StatusCodeResult(200);
            }
            catch (Exception)
            {
                return new StatusCodeResult(500);
            }
            
        }

        //update user http://localhost:5002/user/UpdateUserData/?WordsPerMinute=220&Accuracy=100
        [HttpPost]
        [Route("user/UpdateUserData")]
        public IActionResult UpdateUserData([FromBody]User user, double WordsPerMinute, double Accuracy)
        {
            try
             {
                
                user = _userService.FindUserByName(user.Username);
                user.TypingTests += 1 ;
                if (user.TypingTests == 1)
                {
                    user.BestWpm = WordsPerMinute;
                    user.AverageWpm = WordsPerMinute;
                }
                else if (user.BestWpm < WordsPerMinute)
                {
                    user.BestWpm = WordsPerMinute;
                    user.AverageWpm = ((user.AverageWpm * (user.TypingTests - 1)) + WordsPerMinute) / user.TypingTests;
                }
                var test = new User.Test();
                test.WordsPerMinute = WordsPerMinute;
                test.Accuracy = Accuracy;
                user.tests.Add(test);
                if(_userService.UpdateUser(user))
                    return new StatusCodeResult(200);
                else
                    return new StatusCodeResult(400);
            }
            catch (Exception)
            {
                return new StatusCodeResult(500);
            }
        }
        [HttpGet]
        [Route("user/GetUserRank")]
        public IActionResult GetUserRank(string username)
        {
            try
            {
                User user = _userService.FindUserByName(username);
                return new OkObjectResult(_userService.GetUserRank(user));
            }
            catch (Exception)
            {
                return new StatusCodeResult(500);
            }
        }
        [HttpPost]
        [Route("user/UpdateProfilePicture")]
        //http://localhost:5002/user/UpdateProfilePicture/?profileImageLink=?
        public IActionResult UpdateProfilePicture([FromBody] User user, string profileImageLink)
        {
            try
            {
                user = _userService.FindUserByName(user.Username);
                user.ProfileImage = profileImageLink;

               if(_userService.UpdateUser(user))
                    return new StatusCodeResult(200);
               else
                    return new StatusCodeResult(400);
            }
            catch (Exception)
            {
                return new StatusCodeResult(500);
            }
        }
        //http://localhost:5002/user/GetUserData/?username=ronald2
        [HttpGet]
        [Route("user/GetUserData")]
        public IActionResult GetUserData(string username)
        {
            try
            {
                User userToGet = _userService.FindUserByName(username);
                return new OkObjectResult(userToGet);
            }
            catch (Exception)
            {
                return new StatusCodeResult(500);
            }
        }

        [HttpGet]
        [Route("user/GetLeadboard")]
        public IActionResult GetLeadboard()
        {
            try
            {
                var leaderboard = _userService.GetLeaderboardOfUsers();
                if(leaderboard== null)
                    return new StatusCodeResult(500);
                else
                     return new OkObjectResult(leaderboard.ToArray());
            }
            catch(Exception)
            {
                return new StatusCodeResult(500);
            }
        }
    }
}
