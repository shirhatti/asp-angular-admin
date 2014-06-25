using Admin.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Microsoft.Owin;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;

namespace Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : ApiController
    {

        private ApplicationUserManager _userManager;

        public UsersController()
        {
        }

        public UsersController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        public ApplicationUserManager UserManager {
            get
            {
                return _userManager ?? HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }


        // GET api/users
        public List<dynamic> Get()
        {
            var users = new List<dynamic>();
            foreach (var user in UserManager.Users)
            {
                users.Add(new
                {
                    id = user.Id,
                    email = user.Email,
                    first_name = user.FirstName,
                    last_name = user.LastName
                });
            }
            return users;
        }

        // GET: api/Users/5
        public dynamic Get(string id)
        {
            var user = UserManager.FindById(id);
            return new
            {
                id = user.Id,
                email = user.Email,
                first_name = user.FirstName,
                last_name = user.LastName,
                locked = user.LockoutEnabled,
                locked_end = user.LockoutEndDateUtc,
                email_confirmed = user.EmailConfirmed,
            };
        }

        // POST: api/Users
        public dynamic Post(User value)
        {
            var user = new ApplicationUser();
            user.UserName = value.email;
            user.FirstName = value.first_name;
            user.LastName = value.last_name;
            user.Email = value.email;
            var result = UserManager.Create(user, Faker.StringFaker.Alpha(5).ToUpper() + Faker.StringFaker.Alpha(5) + Faker.StringFaker.Numeric(5) + Faker.StringFaker.SelectFrom(3, "!@#$%^&*()"));

            if (result.Succeeded)
            {
                return new
                {
                    id = user.Id,
                    email = user.Email,
                    first_name = user.FirstName,
                    last_name = user.LastName
                };
            }
            else
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }
        [HttpPost]
        // POST: api/Users/5/ResetPassword
        public async Task ResetPassword(string id)
        {
            ApplicationUser user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            string code = UserManager.GeneratePasswordResetToken(user.Id);
            var callbackUrl = Url.Content(string.Format("~/Account/ResetPassword?userId={0}&code={1}", user.Id, code));

            await UserManager.SendEmailAsync(user.Id, "Reset Password", "Your admin has forced you to reset your password. Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
        }

        // PUT: api/Users/5
        public dynamic Put(string id,User value)
        {
            ApplicationUser update = UserManager.FindById(id);
            if (update == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            update.FirstName = value.first_name;
            update.LastName = value.last_name;
            update.Email = value.email;
            update.LockoutEnabled = value.locked;
            update.EmailConfirmed = value.email_confirmed;
            update.UserName = value.email;

            var result = UserManager.Update(update);
            if (result.Succeeded)
            {
                return new
                {
                    id = update.Id,
                    email = update.Email,
                    first_name = update.FirstName,
                    last_name = update.LastName,
                    locked = update.LockoutEnabled,
                    email_confirmed = update.EmailConfirmed
                };
            }
            else
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }

        }

        // DELETE: api/Users/5
        public void Delete(string id)
        {
            ApplicationUser user = UserManager.FindById(id);
            if (user == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            UserManager.Delete(user);
        }
    }

    public class User
    {
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string id { get; set; }
        public string email { get; set; }
        public bool locked { get; set; }
        public bool email_confirmed { get; set; }
    }

}
