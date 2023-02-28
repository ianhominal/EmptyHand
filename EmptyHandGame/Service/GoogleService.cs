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

namespace Service
{
    public class GoogleService
    {
        UserCredential? credential;

        public async Task<Person> GoogleLogin()
        {
            using (var stream = new FileStream("client_secrets.json", FileMode.Open, FileAccess.Read))
            {
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    new[] { PeopleServiceService.Scope.UserinfoProfile, PeopleServiceService.Scope.UserinfoEmail },
                    "user",
                    CancellationToken.None,
                    new FileDataStore("Drive.Auth.Store"));
            }

            // Create the service.
            var service = new PeopleServiceService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "EmptyHand",
            });

            var peopleRequest = service.People.Get("people/me");
            peopleRequest.RequestMaskIncludeField = new List<string>() {
            "person.names" ,
            "person.photos",
            "person.emailAddresses",
            "person.names"  };

            var profile =await peopleRequest.ExecuteAsync();


            return profile;
        }


        public void GoogleLogout()
        {
            if (credential != null)
            {
                // Invalidar la sesión actual
                credential?.RevokeTokenAsync(CancellationToken.None).Wait();

                // Eliminar el token de acceso
                new FileDataStore("Drive.Auth.Store").ClearAsync().Wait();
            }
        }

    }
}
