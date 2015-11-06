using System;
using Microsoft.Owin.Security;

namespace Owin.Security.Keycloak.Embeddable.Owin
{
    internal class EmbeddedStartup
    {
        public void Configuration(IAppBuilder app)
        {
            var signInAuthType = Guid.NewGuid().ToString();
            app.Use(typeof (EmbeddedAuthenticationMiddleware), app, new EmbeddedAuthenticationOptions(signInAuthType));
            app.SetDefaultSignInAsAuthenticationType(signInAuthType);

            var options = KeycloakIdentityManager.BackchannelGetCurrentOptions();
            options.SignInAsAuthenticationType = signInAuthType;
            app.UseKeycloakAuthentication(options);
        }
    }
}
