
# Distributed Dynamic Configuraiton Sample
A sample .net core repository demonstrates dynamic distributed cache implementation. The diagram below shows the way it works. **Instant cache invalidation** is implemented by using a pubsub server. So, **no need for cache invalidation interval**.

![alt text](https://raw.githubusercontent.com/ibrahimkalyoncu/distributed-config-sample/master/diagram.jpg)

## Usage

 1. Register services for dependency injection

	    public void ConfigureServices(IServiceCollection services)
		{
		    services.AddMvc();
			...
			
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
			...
            app.UseConfiguration();
        }
3. Inject into any controller and use

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

Then you can explore **storefront** on http://127.0.0.1:57001 and **backoffice** on http://127.0.0.1:57002 . You will find three configurations with default values named **FooCount**, **FooEnabled** and **FooString** listed on storefront. Once you create those three configurations on backoffice, you will see the right values on storefront.

After you created the three config, you can stop **mongo** container on docker cli and storefront will still be working from **cache**. 
