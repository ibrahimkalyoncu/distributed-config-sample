
# Distributed Dynamic Configuraiton Sample
A sample .net core repository demonstrates dynamic distributed cache implementation. The diagram below shows the way it works. **Instant cache invalidation** is implemented by using a pubsub server. So, **no need for cache invalidation interval**.

![alt text](https://raw.githubusercontent.com/ibrahimkalyoncu/distributed-config-sample/master/diagram.jpg)

## Usage

 1. Register services for dependency injection

	    public void ConfigureServices(IServiceCollection services)
		{
		    services.AddMvc();
						
		    //Register services for dependency injection
		    services.AddConfiguration();
		}
2. Configure application to subscribe/publish to cache invalidation pubsub channel

	    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
            
            //Configure application to subscribe/publish to cache invalidation pubsub channel
            app.UseConfiguration();
        }
