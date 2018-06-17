using System.Threading.Tasks;
using ConfigurationProvider.Interface;
using Microsoft.AspNetCore.Mvc;
using Storefront.Models;

namespace Storefront.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfigurationProvider _configurationProvider;

        public HomeController(IConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        public async Task<IActionResult> Index()
        {
            var fooCount = await _configurationProvider.GetAsync<int>("FooCount");
            var fooEnabled = await _configurationProvider.GetAsync<bool>("FooEnabled");
            var fooString = await _configurationProvider.GetAsync<string>("FooString");

            return View(new HomeIndexModel
            {
                FooCount = fooCount,
                FooEnabled = fooEnabled,
                FooString = fooString,
            });
        }
    }
}
