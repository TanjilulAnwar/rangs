using ETS.DataAccess.Repository.IRepository;
using ETS.Models.Models.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ETS.Controllers
{
    public class AuthController : BaseController
    {
        private readonly UserManager<AppUser> userManager;
        private readonly IConfiguration _configuration;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IUnitOfWork _unitOfWork;

        public AuthController(IUnitOfWork unitOfWork, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            _configuration = configuration;
            _unitOfWork = unitOfWork;
        }


        //[Authorize(Roles = UserRoles.SYSADMIN + "," + UserRoles.ADMIN)]
        [HttpPost]
        [Route("~/SignUp/")]
        public async Task<IActionResult> Register([FromBody] Registration registration)
        {

            try
            {

               if(registration.password != registration.password_retype)
                {
                    throw new Exception("Password don't match!");
                }


                if (registration.phone_number.Contains("[a-zA-Z]+") || registration.phone_number.Length < 11)
                {
                    throw new Exception("Phone No. Invalid!");
                }



                var userExist = await userManager.FindByEmailAsync(registration.email);
                if (userExist != null)
                {
                    throw new Exception("User of similar email already exists!");
                }   

                userExist = await userManager.FindByNameAsync(registration.phone_number);
                if (userExist != null)
                {
                    throw new Exception("User of similar phone no. already exists");
                }
                    

                AppUser user = new AppUser()
                {

                    Email = registration.email,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = registration.last_name+RandomNumber(8),
                    PhoneNumber = registration.phone_number,
                    EmailConfirmed= false



                };

                var result = await userManager.CreateAsync(user, registration.password);

                if (!result.Succeeded)
                {
                    return Json(new { success = false, message = "Invalid Input!" });
                }

                if (!await roleManager.RoleExistsAsync(Roles.USER))
                    await roleManager.CreateAsync(new IdentityRole(Roles.USER));

                if (await roleManager.RoleExistsAsync(Roles.USER))
                {
                    await userManager.AddToRoleAsync(user, Roles.USER);
                }

                registration.user_id = user.Id;
               await _unitOfWork.Registration.AddAsync(registration);
               await  _unitOfWork.SaveAsync();

                return Json(new { success = true, message = "Registration Successful!" });
            }
            catch(Exception e)
            {
                return Json(new { success = false, message = e.Message });
            }




        }






        [HttpPost]
        [Route("~/Login/")]
        public async Task<IActionResult> Login([FromBody] Login login)
        {


            try
            {
                var user = await userManager.FindByEmailAsync(login.email);
                if (user != null && await userManager.CheckPasswordAsync(user, login.password))
                {
                    var userRoles = await userManager.GetRolesAsync(user);
                    var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("UserID",user.Id)

                };


                foreach (var userRole in userRoles)
                    {
                        authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                    }

                    var authSigninKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
                    var token = new JwtSecurityToken(
                        issuer: _configuration["JWT:ValidIssuer"],
                        audience: _configuration["JWT:ValidAudience"],
                        expires: DateTime.Now.AddHours(24),
                        claims: authClaims,
                        signingCredentials: new SigningCredentials(authSigninKey, SecurityAlgorithms.HmacSha256)
                        );



                    
                    return Ok(new
                    {
                        success = true,
                        token = new JwtSecurityTokenHandler().WriteToken(token)

                    }); 
                }
                return Json(new { success = false, message = "Invalid email or password !" });
            }
            catch(Exception e)
            {
                return Json(new { success = false, message = e.Message });
            }

        }



        [HttpGet]
        [Route("~/Verify/Token")]
        public async Task<IActionResult> GetByToken()
        {
            
            try
            {
               
                var userID = GetUserId();
                if (userID == null)
                {
                    return Json(new { success = false, message = "No data found !" });
                }
                var AspUser = await userManager.FindByIdAsync(userID);
                Registration registration = await _unitOfWork.Registration.GetFirstOrDefaultAsync(u => u.user_id == userID);
                var user = new
                {
                    first_name = registration.first_name,
                    last_name = registration.last_name,
                    email = registration.email,
                    phone_number = registration.phone_number,
                    address = registration.address,
                    user_id = registration.user_id,
                    email_confirmed = AspUser.EmailConfirmed

                };
              
                return Json(new { success = true, message = user });

            }
            catch
            {
                return Json(new { success = false, message = "Failed!" });
            }

        }





        [HttpGet]
        [Route("~/Send/VerificationEmail")]
        public async Task<IActionResult> GetByEmail(string user_id)
        {

            try
            {
                var userID = user_id;
                if (userID == null)
                {
                    return Json(new { success = false, message = "No data found !" });
                }
                var AspUser = await userManager.FindByIdAsync(userID);
                Registration registration = await _unitOfWork.Registration.GetFirstOrDefaultAsync(u => u.user_id == userID);

                using SmtpClient email = new SmtpClient
                {
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    EnableSsl = true,
                    Host = "smtp.gmail.com",
                    Port = 587,
                    Credentials = new NetworkCredential("ibumken@gmail.com", "7thtreaty")

                };

                string Vtoken = RandomStuff(50);
                registration.verification = Vtoken;
                _unitOfWork.Registration.Update(registration);
                await  _unitOfWork.SaveAsync();

                string link = "http://localhost:23493/verifyEmail?user="+ AspUser.Id+ "&Vtoken=" + Vtoken;
                string subject = "Verify Email";
                string body = @"<html><body>  Please Verify your email by clicking this 
                                <a href=""\" + link + "\"> link</a></body></html>";

                email.Send("ibumken@gmail.com", registration.email, subject, body);

                return Json(new { success = true, message = "email has been sent!" });

                //return new ContentResult
                //{
                //    ContentType = "text/html",
                //    StatusCode = (int)HttpStatusCode.OK,
                //    Content = "<html><body>Welcome</body></html>"
                //};

            }
            catch(Exception e)
            {
                return Json(new { success = false, message = e.Message });
            }

        }



        [HttpGet]
        [Route("~/verifyEmail")]
        public async Task<IActionResult> verifyEmail(string Vtoken , string user)
        {

            try
            {
               
                Registration registration = await _unitOfWork.Registration.GetFirstOrDefaultAsync(u => u.user_id == user && u.verification == Vtoken);
                if(registration == null)
                {
                    return Json(new { success = false, message = "The link has expired!" });
                }

                registration.verification = null;
                _unitOfWork.Registration.Update(registration);
                await _unitOfWork.SaveAsync();


                var AspUser = await userManager.FindByIdAsync(user);
               AspUser.EmailConfirmed = true;
               await userManager.UpdateAsync(AspUser);



                return Json(new { success = true, message = "email verified!" });

            }
            catch
            {
                return Json(new { success = false, message = "Failed!" });
            }

        }






    }
}
