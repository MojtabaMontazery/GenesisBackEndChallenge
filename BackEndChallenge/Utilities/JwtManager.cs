using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BackEndChallenge.Utilities
{
    public static class JwtManager
    {
        /// <summary>
        /// Use the below code to generate symmetric Secret Key
        ///     var hmac = new HMACSHA256();
        ///     var key = Convert.ToBase64String(hmac.Key);
        /// </summary>
        private static string Secret
        {
            get
            {
                return (new AppConfiguration()).JWTSecret;
            }
        }


        public static string GenerateToken(string username)
        {
            
            var symmetricKey = Convert.FromBase64String(Secret);
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                        {
                            new Claim(ClaimTypes.Name, username)
                        }),


                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(symmetricKey), SecurityAlgorithms.HmacSha256Signature)
            };

            SecurityToken stoken = tokenHandler.CreateToken(tokenDescriptor);
            string token = tokenHandler.WriteToken(stoken);

            return token;
        }

        //private static ClaimsPrincipal GetPrincipal(string token)
        //{
        //    try
        //    {
        //        var tokenHandler = new JwtSecurityTokenHandler();
        //        var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

        //        if (jwtToken == null)
        //            return null;

        //        var symmetricKey = Convert.FromBase64String(Secret);

        //        var validationParameters = new TokenValidationParameters()
        //        {
        //            RequireExpirationTime = false,
        //            ValidateIssuer = false,
        //            ValidateAudience = false,
        //            IssuerSigningKey = new SymmetricSecurityKey(symmetricKey)
        //        };

        //        SecurityToken securityToken;
        //        var principal = tokenHandler.ValidateToken(token, validationParameters, out securityToken);
        //        return principal;
        //    }

        //    catch (Exception)
        //    {
        //        return null;
        //    }
        //}

        //public static bool ValidateToken(string token, out string username)
        //{
        //    username = null;

        //    var simplePrinciple = GetPrincipal(token);
        //    var identity = simplePrinciple.Identity as ClaimsIdentity;

        //    if (identity == null)
        //        return false;

        //    if (!identity.IsAuthenticated)
        //        return false;

        //    var usernameClaim = identity.FindFirst(ClaimTypes.NameIdentifier);
        //    username = usernameClaim?.Value;

        //    if (string.IsNullOrEmpty(username))
        //        return false;


        //    return true;
        //}
    }
}
