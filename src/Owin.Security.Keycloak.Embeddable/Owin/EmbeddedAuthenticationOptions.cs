using Microsoft.Owin.Security;

namespace Owin.Security.Keycloak.Embeddable.Owin
{
    internal class EmbeddedAuthenticationOptions : AuthenticationOptions
    {
        public EmbeddedAuthenticationOptions(string authenticationType) : base(authenticationType)
        {
        }
    }
}
