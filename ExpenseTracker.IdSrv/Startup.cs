using System;
using ExpenseTracker.IdSrv.Config;
using IdentityServer3.Core.Configuration;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ExpenseTracker.IdSrv.Startup))]
namespace ExpenseTracker.IdSrv
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.Map("/identity", idsrvApp =>
            {
                idsrvApp.UseIdentityServer(new IdentityServerOptions
                {
                    SiteName = "Embedded IdentityServer",
                    IssuerUri = ExpenseTrackerConstants.IdSrvIssuerUri,

                    Factory = new IdentityServerServiceFactory()
                        .UseInMemoryUsers(Users.Get())
                        .UseInMemoryClients(Clients.Get())
                        .UseInMemoryScopes(Scopes.Get()),

                    SigningCertificate = LoadCertificate(),
                });
            });

            //ConfigureAuth(app);
        }

        X509Certificate2 LoadCertificate()
        {
            return new X509Certificate2(
                $"{AppDomain.CurrentDomain.BaseDirectory}\\bin\\idsrv3test.pfx","idsrv3test");
        }
    }
}
