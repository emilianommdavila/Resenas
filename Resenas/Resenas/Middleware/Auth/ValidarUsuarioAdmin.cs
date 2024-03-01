using System.ComponentModel.DataAnnotations;

namespace Resenas.Middleware.Auth
{
    public class ValidarUsuarioAdmin
    {
        [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Method, AllowMultiple = false)]
        public class ValidateAdminUserAttribute : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                var token = value as string;
                if (string.IsNullOrWhiteSpace(token))
                {
                    return new ValidationResult("Unauthorized");
                }

                var tokenService = (TokenService)validationContext.GetService(typeof(TokenService));
                try
                {
                    tokenService.ValidateAdmin(token);
                    return ValidationResult.Success;
                }
                catch (UnauthorizedException)
                {
                    return new ValidationResult("Unauthorized");
                }
            }
        }
    }
}
