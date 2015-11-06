using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;

namespace Owin.Security.Keycloak.Embeddable.Owin
{
    internal class EmbeddedAuthenticationHandler : AuthenticationHandler<EmbeddedAuthenticationOptions>
    {
        protected override Task<AuthenticationTicket> AuthenticateCoreAsync()
        {
            return KeycloakIdentityManager.IsSignedIn()
                ? Task.FromResult(new AuthenticationTicket(KeycloakIdentityManager.GetCurrentIdentity(),
                    new AuthenticationProperties()))
                : null;
        }

        public override async Task<bool> InvokeAsync()
        {
            var secret = Request.Query.Get("secret");
            var requestType = Request.Query.Get("request").ToLowerInvariant();

            if (secret != KeycloakIdentityManager.BackchannelGetCurrentSecret())
            {
                Response.StatusCode = (int) HttpStatusCode.Forbidden;
                return true;
            }

            var ticket = await AuthenticateAsync();

            // Load current identity
            if (ticket?.Identity != null)
            {
                Context.Authentication.User = new ClaimsPrincipal(ticket.Identity);
            }

            return false;
        }

        protected override Task ApplyResponseGrantAsync()
        {
            var signin = Helper.LookupSignIn(Options.AuthenticationType);
            var signout = Helper.LookupSignOut(Options.AuthenticationType, Options.AuthenticationMode);

            // Signing out takes precedence
            if (signout != null)
            {
                KeycloakIdentityManager.BackchannelSignout();
            }

            // Signin if not signing out
            if (signin != null)
            {
                KeycloakIdentityManager.BackchannelSetIdentity(signin.Identity);
            }

            return Task.FromResult(0);
        }
    }
}
