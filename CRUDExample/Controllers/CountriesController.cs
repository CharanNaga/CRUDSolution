using Microsoft.AspNetCore.Mvc;
using ServiceContracts;

namespace CRUDExample.Controllers
{
    [Route("[controller]")]
    public class CountriesController : Controller
    {
        private readonly ICountriesUploaderService _countriesUploaderService;
        public CountriesController(ICountriesUploaderService countriesUploaderService)
        {
            _countriesUploaderService = countriesUploaderService;
        }

        [Route("UploadFromExcel")]
        public IActionResult UploadFromExcel()
        {
            return View();
        }

        [Route("UploadFromExcel")]
        [HttpPost]
        public async Task<IActionResult> UploadFromExcel(IFormFile excelFile)
        {
            if(excelFile == null || excelFile.Length == 0)
            {
                ViewBag.ErrorMessage = "Please select an excel file";
                return View();
            }
            if (!Path.GetExtension(excelFile.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                ViewBag.ErrorMessage = "Unsupported file .xlsx file is expected";
                return View();
            }
            int countriesInserted = await _countriesUploaderService.UploadCountriesFromExcelFile(excelFile);
            ViewBag.Message = $"{countriesInserted} Countries inserted";
            return View();
        }
    }
}
