using OfficeOpenXml.Style;
using OfficeOpenXml;
using ServiceContracts;
using ServiceContracts.DTO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class PersonsGetterServiceWithFewExcelFields : IPersonsGetterService
    {
        private readonly PersonsGetterService _personsGetterService;

        public PersonsGetterServiceWithFewExcelFields(PersonsGetterService personsGetterService)
        {
            _personsGetterService = personsGetterService;
        }

        public async Task<List<PersonResponse>> GetAllPersons()
        {
           return await _personsGetterService.GetAllPersons();
        }

        public async Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString)
        {
            return await _personsGetterService.GetFilteredPersons(searchBy, searchString);
        }

        public async Task<PersonResponse?> GetPersonByPersonID(Guid? personID)
        {
            return await _personsGetterService.GetPersonByPersonID(personID);
        }

        public async Task<MemoryStream> GetPersonsCSV()
        {
            return await _personsGetterService.GetPersonsCSV();
        }

        public async Task<MemoryStream> GetPersonsExcel()
        {
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
            {
                ExcelWorksheet excelWorksheet = excelPackage.Workbook.Worksheets.Add("PersonsSheet");
                excelWorksheet.Cells["A1"].Value = "Person Name";
                excelWorksheet.Cells["B1"].Value = "Age";
                excelWorksheet.Cells["C1"].Value = "Gender";

                //Adding Styling for the Header Cells using ExcelRange
                using (ExcelRange headerCells = excelWorksheet.Cells["A1:C1"])
                {
                    headerCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    headerCells.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                    headerCells.Style.Font.Bold = true;
                }
                int row = 2;
                List<PersonResponse> persons = await GetAllPersons();
                foreach (var person in persons)
                {
                    excelWorksheet.Cells[row, 1].Value = person.PersonName;
                    excelWorksheet.Cells[row, 2].Value = person.Age;
                    excelWorksheet.Cells[row, 3].Value = person.Gender;
                    row++;
                }
                excelWorksheet.Cells[$"A1:C{row}"].AutoFitColumns();
                await excelPackage.SaveAsync();
            }
            memoryStream.Position = 0;
            return memoryStream;
        }
    }
}
