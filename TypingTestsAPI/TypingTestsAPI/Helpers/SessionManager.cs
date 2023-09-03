using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TypingTestsAPI.Models;
namespace TypingTestsAPI.Helpers
{
    public class SessionManager
    {
      
            private readonly IHttpContextAccessor _httpContextAccessor;

            public SessionManager(IHttpContextAccessor httpContextAccessor) =>
                _httpContextAccessor = httpContextAccessor;

            public User GetUserSession()
            {
                try
                {
                    return JsonConvert.DeserializeObject<User>(_httpContextAccessor.HttpContext.Session.GetString("user"));
                }
                catch (Exception) { return null; }
            }
            public void removeSession()
            {
                try
                {
                    _httpContextAccessor.HttpContext.Session.Remove("user");
                }
            catch(Exception) { return; }
            }

            public void CreateUserSession(User user)
            {
                try
                {
                    _httpContextAccessor.HttpContext.Session.SetString("user", JsonConvert.SerializeObject(user));
                }
                catch (Exception) { }
            }
       
    }
}
