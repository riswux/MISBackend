using Microsoft.IdentityModel.Tokens;

namespace MISBackend.Middleware
{

    public interface ITokenLifetimeManager
    {
        public bool ValidateTokenLifetime(DateTime? notBefore,
                                           DateTime? expires,
                                           SecurityToken securityToken,
                                           TokenValidationParameters validationParameters);
    }
}
