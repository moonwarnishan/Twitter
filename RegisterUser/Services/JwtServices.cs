namespace RegisterUser.Services
{
    public class JwtServices : IJwtServices
    {
        private readonly UserServices _userService;
        private readonly string? Key;
        public JwtServices(UserServices userService, IConfiguration configuration)
        {
            _userService = userService;
            Key = configuration.GetSection("JWTKey").ToString();
        }

        public async Task<ReturnToken> Authenticate(LoginModel M)
        {

            var tken = new ReturnToken();
            var user = _userService.LoginValidation(M);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            if (user.isBlocked)
            {
                throw new Exception("User is blocked");
            }

            var refreshToken = GenerateRefreshToken();
            user.refreshToken.Token = refreshToken;
            user.refreshToken.ExpireDate = DateTime.Now.AddDays(7);
            await _userService.UpdateAsync(user);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.ASCII.GetBytes(Key);
            var claim = new List<Claim>
            {
                    new Claim(ClaimTypes.Name, user.userName),
                    new Claim(ClaimTypes.Email,user.email),
                    new Claim(ClaimTypes.Role,user.role)
            };
            var claimsIdentity = new ClaimsIdentity(
                claim, CookieAuthenticationDefaults.AuthenticationScheme);
            //set claim when login
            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                IsPersistent = true,
                AllowRefresh = true
            };
            
            
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claim),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(tokenKey),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };


            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            Thread.CurrentPrincipal = claimsPrincipal;

            var token = tokenHandler.CreateToken(tokenDescriptor);
            tken.AccessToken = tokenHandler.WriteToken(token);
            tken.RefreshToken = refreshToken;
            tken.role = user.role;
            tken.userName = user.userName;

            return tken;
        }
        public ReturnToken TokenRefresh(ReturnToken token)
        {
            if (token is null)
            {
                return null;
            }

            string? accessToken = token.AccessToken.ToString();
            string? refreshToken = token.RefreshToken.ToString();
            var principal = GetPrincipalFromExpiredToken(accessToken);
            if (principal == null)
            {
                return null;
            }

            string username = principal.Identity.Name;
            var user = _userService.FindByuserName(username);
            if (user == null || user.refreshToken.Token != refreshToken || user.refreshToken.ExpireDate <= DateTime.Now)
            {
                return null;
            }

            var newAccessToken = CreateToken(principal.Claims.ToList());
            var newRefreshToken = GenerateRefreshToken();
            user.refreshToken.Token = newRefreshToken;
            user.refreshToken.ExpireDate = DateTime.Now.AddDays(7);
            _userService.UpdateAsync(user);
            var tok = new ReturnToken();

            tok.AccessToken = newAccessToken;
            tok.RefreshToken = newRefreshToken;
            return tok;
        }



        private string CreateToken(List<Claim> Claim)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.ASCII.GetBytes(Key);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(Claim),
                Expires = DateTime.UtcNow.AddSeconds(15),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(tokenKey),
                    SecurityAlgorithms.HmacSha256Signature
            )
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tken = tokenHandler.WriteToken(token);
            return tken;
        }




        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key)),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken)
                throw new SecurityTokenException("Invalid token");

            return principal;

        }


        public static string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
