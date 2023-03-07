using Newtonsoft.Json;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Auth.OAuth2.Web;
using Google.Apis.PeopleService.v1;
using Google.Apis.PeopleService.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.IO;
using System.Threading;
using System.Windows;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Google.Apis.Auth;
using static Google.Apis.Auth.GoogleJsonWebSignature;
using Google.Apis.Util;
using System;

namespace Service
{
    public class GoogleService
    {
        UserCredential? credential;

        public async Task<Payload> GoogleLogin()
        {
            var clientSecrets = new ClientSecrets
            {
                ClientId = "35469603597-htfvps87d81eqfe57r9n65g8iejb41fm.apps.googleusercontent.com",
                ClientSecret = "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"
            };

            credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                clientSecrets,
                new[] { PeopleServiceService.Scope.UserinfoProfile, PeopleServiceService.Scope.UserinfoEmail },
                "user",
                CancellationToken.None,
                new FileDataStore("Drive.Auth.Store"));

            try
            {
                if (credential.Token.IsExpired(SystemClock.Default))
                {
                    bool success = await credential.RefreshTokenAsync(CancellationToken.None);
                    if (!success)
                    {
                        GoogleLogout();
                        throw new Exception("Failed to refresh access token.");
                    }
                }
            }
            catch (Exception ex)
            {
                GoogleLogout();
                throw ex;
            }

            var payload = await GoogleJsonWebSignature.ValidateAsync(credential.Token.IdToken.ToString());
            payload.JwtId = credential.Token.IdToken;
            return payload;
        }


        public void GoogleLogout()
        {
            if (credential != null)
            {
                // Invalidar la sesión actual
                credential?.RevokeTokenAsync(CancellationToken.None).Wait();

                // Eliminar el token de acceso
                new FileDataStore("Drive.Auth.Store").ClearAsync().Wait();

                credential = null;
            }
        }

    }
}
