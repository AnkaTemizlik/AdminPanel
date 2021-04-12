using Hangfire.Dashboard;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NLog;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DNA.API.BackgroundServices.Hangfire {

    public class HangfireDashboardJwtAuthorizationFilter : IDashboardAuthorizationFilter {
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        public static readonly string HangfireCookieName = "HangfireCookie";
        private static readonly int _CookieExpirationMinutes = 60 * 24;
        private TokenValidationParameters _tokenValidationParameters;
        private string[] _roles;

        public HangfireDashboardJwtAuthorizationFilter(TokenValidationParameters tokenValidationParameters, List<string> roles = null) {
            this._tokenValidationParameters = tokenValidationParameters;
            this._roles = roles?.ToArray();
        }

        public bool Authorize(DashboardContext context) {
            var httpContext = context.GetHttpContext();

            var access_token = String.Empty;
            var setCookie = false;

            // try to get token from query string
            if (httpContext.Request.Query.ContainsKey("access_token")) {
                access_token = httpContext.Request.Query["access_token"].FirstOrDefault();
                setCookie = true;
            }
            else {
                access_token = httpContext.Request.Cookies[HangfireCookieName];
            }

            if (String.IsNullOrEmpty(access_token)) {
                return false;
            }

            try {
                SecurityToken validatedToken = null;
                JwtSecurityTokenHandler hand = new JwtSecurityTokenHandler();
                var claims = hand.ValidateToken(access_token, this._tokenValidationParameters, out validatedToken);
                var role = claims.FindFirst(ClaimTypes.Role)?.Value;
                if (this._roles == null || role == null)
                    return false;
            }
            catch (Exception e) {
                _logger.Error(e, "Error during dashboard hangfire jwt validation process");
                return false;
            }

            if (setCookie) {
                httpContext.Response.Cookies.Append(HangfireCookieName,
                access_token,
                new CookieOptions() {
                    Expires = DateTime.Now.AddMinutes(_CookieExpirationMinutes)
                });
            }

            return true;
        }
    }
}
