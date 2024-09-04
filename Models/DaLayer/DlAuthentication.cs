using HospitalManagementApi.Models.Balayer;
using MySql.Data.MySqlClient;
using BaseClass;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace HospitalManagementApi.Models.DaLayer
{
    public class DlAuthentication
    {
        ReturnClass.ReturnDataTable dt = new ReturnClass.ReturnDataTable();
        readonly DBConnection db = new();
        Utilities util = new Utilities();
        public async Task<User> AuthenticateUser(string emailid, string password, LoginTrail lt)
        {
            DlCommon dl = new DlCommon();
            User user = await dl.GetUser(emailid, password, isSwsUser: false);

            ReturnClass.ReturnBool rb = new ReturnClass.ReturnBool();

            // return null if user not found
            if (user.userId == 0)
                return null;

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            // var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            ReturnClass.ReturnBool rbKey = util.GetAppSettings("AppSettings", "Secret");
            var key = rbKey.status ? Encoding.ASCII.GetBytes(rbKey.message) : Encoding.ASCII.GetBytes("");

            rbKey = util.GetAppSettings("AppSettings", "SessionDuration");
            var sessionDuration = rbKey.status ? Convert.ToInt16(rbKey.message) : 0;

            List<Claim> claim = new List<Claim>();
            claim.Add(new Claim(ClaimTypes.Role, user.role.ToString()));
            claim.Add(new Claim("userId", user.userId.ToString()));
            // claim.Add(new Claim("isSingleWindowUser", user.isSingleWindowUser.ToString()));

            ClaimsIdentity subject = new ClaimsIdentity(claim.ToArray());
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = subject,
                Expires = DateTime.UtcNow.AddHours(sessionDuration),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.token = tokenHandler.WriteToken(token);

            // remove password before returning
            user.password = null;


            if (user == null)
            {
                lt.isLoginSuccessful = 0;   // LOGIN FAILURE
                lt.loginId = null;
            }
            else
            {
                lt.isLoginSuccessful = 1;    // SUCCESSFULL LOGIN
                lt.loginId = emailid;
            }

            lt.SetAccessMode();
            rb = await dl.InsertLoginTrail(lt);

            return user;
        }

        public async Task<UserResponse> CheckEmail(string emailid)
        {
            DlCommon dl = new DlCommon();
            return await dl.GetUserByEmail(emailid); ;
        }
        public async Task<UserResponse> CheckMobile(string MobileNo)
        {
            DlCommon dl = new DlCommon();
            return await dl.GetUserByMobile(MobileNo); ;
        }

        public async Task<User> AuthenticateUserByOTP(string emailid, string mobileNo, string OTP, string msgId, LoginTrail lt)
        {
            DlCommon dl = new DlCommon();
            User user = await dl.GetUserByOTP(emailid, mobileNo, OTP, msgId);
            // return null if user not found
            if (user.isAuthenticated == false)
            {
                user = new();
                user.message = "Invalid OTP Details.";
                user.isAuthenticated = false;
                return user;
            }

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            // var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            ReturnClass.ReturnBool rbKey = util.GetAppSettings("AppSettings", "Secret");
            var key = rbKey.status ? Encoding.ASCII.GetBytes(rbKey.message) : Encoding.ASCII.GetBytes("");

            rbKey = util.GetAppSettings("AppSettings", "SessionDuration");
            var sessionDuration = rbKey.status ? Convert.ToInt16(rbKey.message) : 0;

            List<Claim> claim = new List<Claim>();
            claim.Add(new Claim(ClaimTypes.Role, user.role.ToString()));
            claim.Add(new Claim("userId", user.userId.ToString()));
            // claim.Add(new Claim("isSingleWindowUser", user.isSingleWindowUser.ToString()));

            ClaimsIdentity subject = new ClaimsIdentity(claim.ToArray());
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = subject,
                Expires = DateTime.UtcNow.AddHours(sessionDuration),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.token = tokenHandler.WriteToken(token);

            // remove password before returning
            user.password = null;


            if (user == null)
            {
                lt.isLoginSuccessful = 0;   // LOGIN FAILURE
                lt.loginId = null;
            }
            else
            {
                lt.isLoginSuccessful = 1;    // SUCCESSFULL LOGIN
                lt.loginId = emailid;
            }

            lt.SetAccessMode();
            await dl.InsertLoginTrail(lt);

            return user;
        }
    }
}
