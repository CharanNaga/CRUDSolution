using CsvHelper;
using CsvHelper.Configuration;
using Entities;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using RepositoryContracts;
using Serilog;
using SerilogTimings;
using ServiceContracts;
using ServiceContracts.DTO;
using System.Drawing;
using System.Globalization;

namespace Services
{
    public class PersonsGetterService : IPersonsGetterService
    {
        private readonly IPersonsRepository _personsRepository;
        private readonly ILogger<PersonsGetterService> _logger;
        private readonly IDiagnosticContext _diagnosticContext;

        public PersonsGetterService(IPersonsRepository personsRepository,ILogger<PersonsGetterService> logger,IDiagnosticContext diagnosticContext)
        {
            _personsRepository = personsRepository;
            _logger = logger;
            _diagnosticContext = diagnosticContext;
        }

        public virtual async Task<List<PersonResponse>> GetAllPersons()
        {
            //log message
            _logger.LogInformation("GetAllPersons() service method");

            var persons = await _personsRepository.GetAllPersons();
            //Converts all persons from "Person" type to "PersonResponse" type.
            //Return all PersonResponse Objects.

            //return _db.Persons.ToList() //Converting linq to entities expression
            //    .Select(p => ConvertPersonToPersonResponse(p)).ToList();

            return persons //By using navigation property so that we can access CountryID and CountryName properties like persons.Country.CountryName
                           //.Select(p => ConvertPersonToPersonResponse(p)).ToList();
                .Select(p => p.ToPersonResponse()).ToList();

            //return _db.sp_GetAllPersons() //using stored procedures to avoid further errors
            //   .Select(p => ConvertPersonToPersonResponse(p)).ToList();
            //   .Select(p => p.ToPersonResponse()).ToList();
        }

        public virtual async Task<PersonResponse?> GetPersonByPersonID(Guid? personID)
        {
            //1. Check personID != null
            if (personID == null)
                return null;

            //2. Get matching person from List<Person> based on personID
            //Person? personsFromList = _db.Persons.FirstOrDefault(p=>p.PersonID == personID);
            Person? personsFromList = await _personsRepository.GetPersonByPersonID(personID.Value);

            //3. Convert matching person object from Person to PersonResponse type
            //4. Return PersonResponse Object
            if (personsFromList == null)
                return null;
            //return ConvertPersonToPersonResponse(personsFromList);
            return personsFromList.ToPersonResponse();
        }

        public virtual async Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString)
        {
            //writing log message
            _logger.LogInformation("GetFilteredPersons() Service method");

            List<Person> allPersons;
            //Recording time taken by this switch case code block to execute
            using (Operation.Time("Time taken for Filtering Persons retrieving from Database"))
            {
                allPersons = searchBy switch
                {
                    nameof(PersonResponse.PersonName) => await _personsRepository.GetFilteredPersons(
                        p => p.PersonName.Contains(searchString)),

                    nameof(PersonResponse.Email) => await _personsRepository.GetFilteredPersons(
                        p => p.Email.Contains(searchString)),

                    nameof(PersonResponse.DateOfBirth) => await _personsRepository.GetFilteredPersons(
                        p => p.DateOfBirth.Value.ToString("yyyy-MM-dd").Contains(searchString)),

                    nameof(PersonResponse.Gender) => await _personsRepository.GetFilteredPersons(
                        p => p.Gender.Contains(searchString)),

                    nameof(PersonResponse.CountryID) => await _personsRepository.GetFilteredPersons(
                        p => p.Country.CountryName.Contains(searchString)),

                    nameof(PersonResponse.Address) => await _personsRepository.GetFilteredPersons(
                        p => p.Address.Contains(searchString)),

                    _ => await _personsRepository.GetAllPersons()
                };
            }
            _diagnosticContext.Set("Persons", allPersons);
            //3. Convert matching persons from Person to PersonResponse type. (Done in switch case).
            //4. Return all matching PersonResponse objects
            return allPersons.Select(temp => temp.ToPersonResponse()).ToList();
        }

        public virtual async Task<MemoryStream> GetPersonsCSV()
        {
            MemoryStream memoryStream = new MemoryStream();
            StreamWriter streamWriter = new StreamWriter(memoryStream);

            CsvConfiguration csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture);

            //Passing Writer obj, Language to identify the ".", "," symbols and leaveOpen parameter to CsvWriter constructor 
            //CsvWriter csvWriter = new CsvWriter(streamWriter,CultureInfo.InvariantCulture,leaveOpen:true);
            CsvWriter csvWriter = new CsvWriter(streamWriter, csvConfiguration);

            //writing headers using Generic of type PersonResponse to WriteHeader() of CsvWriter class.
            //csvWriter.WriteHeader<PersonResponse>(); //PersonID,PersonName,Email,DOB,....
            //writing selective headers manually using WriteField()
            csvWriter.WriteField(nameof(PersonResponse.PersonName));
            csvWriter.WriteField(nameof(PersonResponse.Email));
            csvWriter.WriteField(nameof(PersonResponse.DateOfBirth));
            csvWriter.WriteField(nameof(PersonResponse.Age));
            csvWriter.WriteField(nameof(PersonResponse.Country));
            csvWriter.WriteField(nameof(PersonResponse.Address));
            csvWriter.WriteField(nameof(PersonResponse.ReceiveNewsLetters));

            //moving to next record for writing next set of data
            csvWriter.NextRecord();

            //Retrieving list of persons, and passing the same list obj to the WriteRecords() for writing data
            List<PersonResponse> persons = await GetAllPersons();
            //await csvWriter.WriteRecordsAsync(persons); //1,abc,....
            //manually looping through list for retrieving selected column related data
            foreach (var person in persons)
            {
                csvWriter.WriteField(person.PersonName);
                csvWriter.WriteField(person.Email);
                if (person.DateOfBirth.HasValue)
                    csvWriter.WriteField(person.DateOfBirth.Value.ToString("yyyy-MM-dd"));
                else
                    csvWriter.WriteField("");
                csvWriter.WriteField(person.Age);
                csvWriter.WriteField(person.Country);
                csvWriter.WriteField(person.Address);
                csvWriter.WriteField(person.ReceiveNewsLetters);
                csvWriter.NextRecord();
                csvWriter.Flush();
            }

            //Setting memory stream position to Zero, for writing new data & then returning memorystream
            memoryStream.Position = 0;
            return memoryStream;
        }

        public virtual async Task<MemoryStream> GetPersonsExcel()
        {
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
            {
                ExcelWorksheet excelWorksheet = excelPackage.Workbook.Worksheets.Add("PersonsSheet");
                excelWorksheet.Cells["A1"].Value = "Person Name";
                excelWorksheet.Cells["B1"].Value = "Email";
                excelWorksheet.Cells["C1"].Value = "Date of Birth";
                excelWorksheet.Cells["D1"].Value = "Age";
                excelWorksheet.Cells["E1"].Value = "Gender";
                excelWorksheet.Cells["F1"].Value = "Country";
                excelWorksheet.Cells["G1"].Value = "Address";
                excelWorksheet.Cells["H1"].Value = "Receive News Letters";

                //Adding Styling for the Header Cells using ExcelRange
                using (ExcelRange headerCells = excelWorksheet.Cells["A1:H1"])
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
                    excelWorksheet.Cells[row, 2].Value = person.Email;
                    if (person.DateOfBirth.HasValue)
                        excelWorksheet.Cells[row, 3].Value = person.DateOfBirth.Value.ToString("yyyy-MM-dd");
                    excelWorksheet.Cells[row, 4].Value = person.Age;
                    excelWorksheet.Cells[row, 5].Value = person.Gender;
                    excelWorksheet.Cells[row, 6].Value = person.Country;
                    excelWorksheet.Cells[row, 7].Value = person.Address;
                    excelWorksheet.Cells[row, 8].Value = person.ReceiveNewsLetters;
                    row++;
                }
                excelWorksheet.Cells[$"A1:H{row}"].AutoFitColumns();
                await excelPackage.SaveAsync();
            }
            memoryStream.Position = 0;
            return memoryStream;
        }
    }
}