
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
3. Inject and use

		public  HomeController(IConfigurationProvider configurationProvider)
		{
			_configurationProvider  =  configurationProvider;
		}
		
		public  async  Task<IActionResult> Index()
		{
		    var fooCount =  await  _configurationProvider.GetAsync<int>("FooCount");
		    var fooEnabled =  await  _configurationProvider.GetAsync<bool>("FooEnabled");
		    var fooString =  await  _configurationProvider.GetAsync<string>("FooString");
			...
		}

## Demo
Use **docker-compose** to setup all you need to test the demo applications.

    docker-compose -f docker-compose.yml up -d

Then you can explore **storefront** on http://127.0.0.1:57001 and **backoffice** on http://127.0.0.1:57002 . Storefront's application name is **SERVICE-A** and stored on **appsettings.json**. You will find three configurations with default values named **FooCount**, **FooEnabled** and **FooString** listed on storefront. Once you create those three configurations on backoffice, you will see the right values on storefront. If you create any of these three config with a different application name than SERVICE-A, storefront will not display the values.

After you created the three config, you can stop **mongo** container on docker cli and storefront will still be working from **cache**.

    docker container stop mongo

While mongo is unreachable storefront will keep working but backoffice will crash as expected. Please be sure of you created the three config mentioned and refresh storefront to cache those values before stopping mongo container.

Also there are mongo and redis gui applications on docker compose file. You can use Redis Commander on http://127.0.0.1:57003/ Mongo Express on http://127.0.0.1:57004/ . If you want to be sure of caching works; you can check query stats under server status tab on Mongo Express.

Cheers!
