using System.Linq;
using System.Threading.Tasks;
using Backoffice.Models;
using ConfigurationProvider.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Backoffice.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfigurationDatasource _configurationDatasource;

        public HomeController(IConfigurationDatasource configurationDatasource)
        {
            _configurationDatasource = configurationDatasource;
        }

        public async Task<IActionResult> Index()
        {
            var configs = await _configurationDatasource.GetAllAsync();
            return View(new HomeIndexModel
            {
                ConfigList = configs.Select(c => new ConfigModel
                {
                    Id = c.Id,
                    IsActive = c.IsActive,
                    Name = c.Name,
                    Type = c.Type,
                    Value = c.Value
                }).ToList()
            });
        }
     }
}
