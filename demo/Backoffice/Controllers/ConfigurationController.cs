using System.Threading.Tasks;
using Backoffice.Models;
using ConfigurationProvider;
using ConfigurationProvider.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Backoffice.Controllers
{
    public class ConfigurationController : Controller
    {
        private readonly IConfigurationDatasource _configurationDatasource;

        public ConfigurationController(IConfigurationDatasource configurationDatasource)
        {
            _configurationDatasource = configurationDatasource;
        }

        public async Task<IActionResult> Add([FromBody] ConfigModel config)
        {
            var isSaved = await _configurationDatasource.UpsertAsync(new Config
            {
                Id = config.Id,
                Name = config.Name,
                Value = config.Value,
                IsActive = config.IsActive,
                Type = config.Type
            });

            return Json(new { Result = isSaved });
        }

        public async Task<IActionResult> Delete(string id)
        {
            var isDeleted = await _configurationDatasource.DeleteAsync(id);
            return Json(new { Result = isDeleted });
        }

        public async Task<IActionResult> List()
        {
            return Json(new { Result = await _configurationDatasource.GetAllAsync() });
        }
    }
}