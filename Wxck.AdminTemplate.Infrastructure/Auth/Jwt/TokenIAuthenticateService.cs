using System;
using System.Linq;
using System.Text;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Caching.Memory;
using Wxck.AdminTemplate.CrossCutting.Auth.Jwt;

namespace Wxck.AdminTemplate.Infrastructure.Auth.Jwt {

    public class TokenIAuthenticateService : IAuthenticateService {
        private readonly IMemoryCache _memoryCache;
        private readonly TokenManagement _tokenManagement;

        public TokenIAuthenticateService(IOptions<TokenManagement> tokenManagement,
            IMemoryCache memoryCache) {
            _memoryCache = memoryCache;
            _tokenManagement = tokenManagement.Value;
        }

        public bool IsAuthenticated(LoginRequestDto request, out string token) {
            token = string.Empty;
            var claim = new Claim[] {
                new(ClaimTypes.Name,request.UserCode??string.Empty)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenManagement.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var jwtSecurityToken = new JwtSecurityToken(
                _tokenManagement.Issuer,// Issuer 颁发者，通常为STS服务器地址
                _tokenManagement.Audience,// Audience Token的作用对象，也就是被访问的资源服务器授权标识
                claim,
                DateTime.Now,  //Token生效时间，在此之前不可用
                DateTime.Now.AddMinutes(_tokenManagement.AccessExpiration), //Token过期时间，在此之后不可用
                creds);
            token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            _memoryCache.Set(token,
                $"Bearer_{request.UserCode}",
                TimeSpan.FromMinutes(_tokenManagement.AccessExpiration));
            return true;
        }

        public bool IsTokenBlacklisted(string token) {
            var s = _memoryCache.Get<string>(token);
            return string.IsNullOrEmpty(s);
        }
    }

    public class TokenManagement {

        /// <summary>
        /// 用于加密的key
        /// </summary>
        public string Secret { get; set; } = string.Empty;

        /// <summary>
        /// Token是谁颁发的
        /// </summary>
        public string Issuer { get; set; } = string.Empty;

        /// <summary>
        /// Token给那些客户端去使用
        /// </summary>
        public string Audience { get; set; } = string.Empty;

        public int AccessExpiration { get; set; }

        public int RefreshExpiration { get; set; }
    }
}