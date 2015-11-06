using System;
using System.Net;
using System.Net.Sockets;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using Owin.Security.Keycloak.Embeddable.Owin;

namespace Owin.Security.Keycloak.Embeddable
{
    public static class KeycloakIdentityManager
    {
        private const string DefaultBaseUrl = "http://localhost";
        private static readonly Mutex ObjectLock = new Mutex();
        private static IDisposable _webApp;
        private static bool _webAppStarted = false;
        private static bool _signedIn = false;
        private static KeycloakAuthenticationOptions _options = null;
        private static ClaimsIdentity _identity = null;
        private static string _secret = null;

        public static void Preload(KeycloakAuthenticationOptions options)
        {
            Preload(options, DefaultBaseUrl + ":" + FreeTcpPort() + "/");
        }

        public static void Preload(KeycloakAuthenticationOptions options, string url)
        {
            lock (ObjectLock)
            {
                if (_webAppStarted) return;
                _options = options;
                _secret = Guid.NewGuid().ToString();
                _webApp = WebApp.Start<EmbeddedStartup>(url);
                _webAppStarted = true;
            }
        }

        public static bool IsSignedIn()
        {
            lock (ObjectLock)
            {
                return _signedIn;
            }
        }

        public static async Task<bool> SignIn()
        {
            lock (ObjectLock)
            {
                if (!_webAppStarted) return false;
                // TODO: SignIn & avoid deadlock
                return false;
            }
        }

        public static async Task<ClaimsIdentity> GetIdentity()
        {
            lock (ObjectLock)
            {
                if (!_webAppStarted || !_signedIn) return null;
                // TODO: Update identity & avoid deadlock
                return _identity;
            }
        }

        public static async Task SignOut()
        {
            lock (ObjectLock)
            {
                // TODO: SignOut & avoid deadlock
            }
        }

        #region Backchannel Server Functions

        internal static void BackchannelSetIdentity(ClaimsIdentity identity)
        {
            lock (ObjectLock)
            {
                _identity = identity;
            }
        }

        internal static void BackchannelSignout()
        {
            lock (ObjectLock)
            {
                _signedIn = false;
                _identity = null;
            }
        }

        internal static KeycloakAuthenticationOptions BackchannelGetCurrentOptions()
        {
            lock (ObjectLock)
            {
                return _options;
            }
        }

        internal static string BackchannelGetCurrentSecret()
        {
            lock (ObjectLock)
            {
                return _secret;
            }
        }

        internal static ClaimsIdentity GetCurrentIdentity()
        {
            lock (ObjectLock)
            {
                return _identity;
            }
        }

        #endregion
        
        public static void Dispose()
        {
            lock (ObjectLock)
            {
                if (!_webAppStarted) return;
                _webApp.Dispose();
                _options = null;
                _signedIn = false;
                _webAppStarted = false;
            }
        }

        private static int FreeTcpPort()
        {
            var l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            var port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();
            return port;
        }
    }
}
