using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using Plugin;
namespace AppPartsSample
{
    public class Program
    {
        public static IHostingEnvironment Environment { get; set; }

        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    Environment = hostingContext.HostingEnvironment;
                })
                .ConfigureServices(services =>
                {
                    var pluginAssembly = Assembly.Load(new AssemblyName("Plugin"));
                    services.AddMvc()
                        .AddApplicationPart(pluginAssembly)
                        .ConfigureApplicationPartManager(p =>
                            {
                                var dependentLibrary = p.ApplicationParts
                                    .FirstOrDefault(part => part.Name == "DependentLibrary");
                                if (dependentLibrary != null)
                                {
                                    p.ApplicationParts.Remove(dependentLibrary);
                                }
                            })
                        .ConfigureApplicationPartManager(p => p.FeatureProviders.Add(new GenericControllerFeatureProvider()));
						
					services.Configure<RazorViewEngineOptions>(options => {
						options.FileProviders.Add(new EmbeddedFileProvider(typeof(HelloController)
							.GetTypeInfo().Assembly));
					});
                })
                .Configure(app =>
                {
                    if (Environment.IsDevelopment())
                    {
                        app.UseDeveloperExceptionPage();
                        app.UseBrowserLink();
                    }
                    else
                    {
                        app.UseExceptionHandler("/Home/Error");
                    }

                    app.UseStatusCodePages();
                    app.UseStaticFiles();
                    app.UseMvcWithDefaultRoute();
                })
                .Build();
    }
}
