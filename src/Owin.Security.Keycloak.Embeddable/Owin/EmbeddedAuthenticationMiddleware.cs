using Microsoft.Owin;
using Microsoft.Owin.Security.Infrastructure;

namespace Owin.Security.Keycloak.Embeddable.Owin
{
    internal class EmbeddedAuthenticationMiddleware : AuthenticationMiddleware<EmbeddedAuthenticationOptions>
    {
        public EmbeddedAuthenticationMiddleware(OwinMiddleware next, EmbeddedAuthenticationOptions options)
            : base(next, options)
        {
        }

        protected override AuthenticationHandler<EmbeddedAuthenticationOptions> CreateHandler()
        {
            return new EmbeddedAuthenticationHandler();
        }
    }
}
