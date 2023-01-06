using Microsoft.IdentityModel.Tokens;
using QuizAppApi.Infrastructure.DBContext;
using QuizAppApi.Infrastructure.ServiceResponse;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using QuizAppApi.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace QuizAppApi.Services.Authentication
{
    public class AuthServices : IAuthServices
    {
        private readonly DbInfo _dbInfo;

        public readonly IConfiguration _config;

        public AuthServices(DbInfo dbInfo, IConfiguration config)
        {
            _dbInfo = dbInfo;
            _config = config;
        }


        public async Task<ServiceResponse<string>> Login(String email, string Password)
        {
            var response = new ServiceResponse<string>();
            var user = await _dbInfo.UsersTable.FirstOrDefaultAsync(u => u.Email.ToLower().Equals(email));
            //User name auth validation
            if (user == null)
            {
                response.Success = false;
                response.Message = "Company not found!";
                //new Error(response.Message + " Company = " + CompanyName + " not present in Database");
            }
            //Password auth 
            else if (!VerifyPasswordHash(Password, user.PasswordHash, user.PasswordSalt))
            {
                response.Success = false;
                response.Message = "Wrong Password";
                //new Error(response.Message + " Company = " + CompanyName + " present in Database. But Password did not match");
            }
            else
            {
                response.Message = "Logged In";
                response.Data = CreateToken(user);
            }

            return response;
        }

        public async Task<ServiceResponse<int>> Register(User user, string Password)
        {
            ServiceResponse<int> response = new ServiceResponse<int>();
            if (await UserAlreadyExist(user.Email))
            {
                response.Success = false;
                response.Message = "User Already Exists";
                return response;
            }
            CreatePasswordHash(Password, out byte[] PasswordHash, out byte[] PasswordSalt);

            user.PasswordSalt = PasswordSalt;
            user.PasswordHash = PasswordHash;

            _dbInfo.UsersTable.Add(user);
            await _dbInfo.SaveChangesAsync();

            response.Data = user.UserId;
            return response;
        }

        public async Task<bool> UserAlreadyExist(String email)
        {
            if (await _dbInfo.UsersTable.AnyAsync(u => u.Email.ToLower().Equals(email)))
            {
                return true;
            }
            return false;
        }

        private void CreatePasswordHash(string Password, out byte[] PasswordHash, out byte[] PasswordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                PasswordSalt = hmac.Key;
                PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(Password));
            }
        }
        private bool VerifyPasswordHash(string Password, byte[] PasswordHash, byte[] PasswordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(PasswordSalt))
            {
                var ComputeHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(Password));
                return (ComputeHash.SequenceEqual(PasswordHash));
            }
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier,user.UserId.ToString()),
                new Claim(ClaimTypes.Name,user.Name),
                new Claim(ClaimTypes.Role,user.Role.ToString())
            };
            SymmetricSecurityKey Key = new SymmetricSecurityKey(System.Text.Encoding.UTF8
                .GetBytes(_config.GetSection("AppSettings:Token").Value));

            SigningCredentials Creds = new SigningCredentials(Key, SecurityAlgorithms.HmacSha512Signature);

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = (DateTime.Now.AddMinutes(15)),
                SigningCredentials = Creds
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
