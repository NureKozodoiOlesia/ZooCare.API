using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ZooCare.API.Data;

namespace ZooCare.API.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private readonly string[]? _allowedRoles;

        public AuthorizeAttribute(params string[]? allowedRoles)
        {
            _allowedRoles = allowedRoles;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            // Перевірка наявності токена
            if (!context.HttpContext.Request.Headers.ContainsKey("Authorization"))
            {
                context.Result = new UnauthorizedObjectResult(new { message = "Токен авторизації відсутній" });
                return;
            }

            var authHeader = context.HttpContext.Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                context.Result = new UnauthorizedObjectResult(new { message = "Невірний формат токена" });
                return;
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();
            
            // Розшифровка токена (для MVP проста перевірка)
            var userInfo = DecodeToken(token);
            if (userInfo == null)
            {
                context.Result = new UnauthorizedObjectResult(new { message = "Невірний токен" });
                return;
            }

            // Додаємо інформацію про користувача до контексту
            context.HttpContext.Items["UserId"] = userInfo.UserId;
            context.HttpContext.Items["UserEmail"] = userInfo.Email;

            // Перевірка ролей, якщо вони вказані
            if (_allowedRoles != null && _allowedRoles.Length > 0)
            {
                // Отримуємо ролі користувача з бази даних
                var userRoles = await GetUserRoles(context, userInfo.UserId);
                
                if (!_allowedRoles.Any(role => userRoles.Contains(role, StringComparer.OrdinalIgnoreCase)))
                {
                    context.Result = new ForbidObjectResult(new { message = "Недостатньо прав доступу" });
                    return;
                }
            }
        }

        private UserTokenInfo? DecodeToken(string token)
        {
            try
            {
                var bytes = Convert.FromBase64String(token);
                var payload = System.Text.Encoding.UTF8.GetString(bytes);
                var parts = payload.Split(':');
                
                if (parts.Length >= 2 && int.TryParse(parts[0], out var userId))
                {
                    return new UserTokenInfo
                    {
                        UserId = userId,
                        Email = parts[1]
                    };
                }
            }
            catch
            {
                // Токен невалідний
            }
            
            return null;
        }

        private async Task<List<string>> GetUserRoles(AuthorizationFilterContext context, int userId)
        {
            var serviceProvider = context.HttpContext.RequestServices;
            var contextDb = serviceProvider.GetRequiredService<Data.ZooContext>();
            
            var userRoles = await contextDb.UserRoles
                .Include(ur => ur.Role)
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.Role!.Name)
                .ToListAsync();
            
            return userRoles;
        }

        private class UserTokenInfo
        {
            public int UserId { get; set; }
            public string Email { get; set; } = string.Empty;
        }
    }

    public class ForbidObjectResult : ObjectResult
    {
        public ForbidObjectResult(object value) : base(value)
        {
            StatusCode = 403;
        }
    }
}

