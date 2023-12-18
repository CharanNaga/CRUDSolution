using CsvHelper;
using CsvHelper.Configuration;
using Entities;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services.Helpers;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Globalization;

namespace Services
{
    public class PersonsService : IPersonsService
    {
        private readonly IPersonsRepository _personsRepository;

        public PersonsService(IPersonsRepository personsRepository)
        {
            _personsRepository = personsRepository;
        }

        public async Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest)
        {
            //1. Check personAddRequest != null
            if (personAddRequest == null)
                throw new ArgumentNullException(nameof(personAddRequest));

            //2. Validate all properties of personAddRequest
            ValidationHelper.ModelValidation(personAddRequest); //validating all properties using Model validations by calling a reusable method.

            //3. Convert personAddRequest to Person type
            Person person = personAddRequest.ToPerson();

            //4. Generate a new PersonID
            person.PersonID = Guid.NewGuid();

            //5. Then add it to the List<Person>
            await _personsRepository.AddPerson(person);

            //_db.sp_InsertPerson(person); //performing insertion using stored procedure

            //6. Return PersonResponse object with generated PersonID.
            //return ConvertPersonToPersonResponse(person);  
            return person.ToPersonResponse();
        }

        public async Task<List<PersonResponse>> GetAllPersons()
        {
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

        public async Task<PersonResponse?> GetPersonByPersonID(Guid? personID)
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

        public async Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString)
        {
            List<Person> allPersons = searchBy switch
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
            //3. Convert matching persons from Person to PersonResponse type. (Done in switch case).
            //4. Return all matching PersonResponse objects
            return allPersons.Select(temp => temp.ToPersonResponse()).ToList();
        }

        public async Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrder)
        {
            if (string.IsNullOrEmpty(sortBy))
                return allPersons;

            List<PersonResponse> sortedPersons = (sortBy, sortOrder) switch
            {
                (nameof(PersonResponse.PersonName), SortOrderOptions.ASC)
                => allPersons.OrderBy(p => p.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.PersonName), SortOrderOptions.DESC)
                => allPersons.OrderByDescending(p => p.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Email), SortOrderOptions.ASC)
                => allPersons.OrderBy(p => p.Email, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Email), SortOrderOptions.DESC)
                => allPersons.OrderByDescending(p => p.Email, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.DateOfBirth), SortOrderOptions.ASC)
                => allPersons.OrderBy(p => p.DateOfBirth).ToList(),

                (nameof(PersonResponse.DateOfBirth), SortOrderOptions.DESC)
                => allPersons.OrderByDescending(p => p.DateOfBirth).ToList(),

                (nameof(PersonResponse.Age), SortOrderOptions.ASC)
                => allPersons.OrderBy(p => p.Age).ToList(),

                (nameof(PersonResponse.Age), SortOrderOptions.DESC)
                => allPersons.OrderByDescending(p => p.Age).ToList(),

                (nameof(PersonResponse.Gender), SortOrderOptions.ASC)
                => allPersons.OrderBy(p => p.Gender, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Gender), SortOrderOptions.DESC)
                => allPersons.OrderByDescending(p => p.Gender, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Country), SortOrderOptions.ASC)
                => allPersons.OrderBy(p => p.Country, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Country), SortOrderOptions.DESC)
                => allPersons.OrderByDescending(p => p.Country, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Address), SortOrderOptions.ASC)
                => allPersons.OrderBy(p => p.Address, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Address), SortOrderOptions.DESC)
                => allPersons.OrderByDescending(p => p.Address, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.ASC)
                => allPersons.OrderBy(p => p.ReceiveNewsLetters).ToList(),

                (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.DESC)
                => allPersons.OrderByDescending(p => p.ReceiveNewsLetters).ToList(),

                _ => allPersons
            };
            return sortedPersons;
        }

        public async Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest)
        {
            //1. Check personUpdateRequest != null
            if (personUpdateRequest == null)
                throw new ArgumentNullException(nameof(Person));

            //2. Validate all properties of personUpdateRequest
            ValidationHelper.ModelValidation(personUpdateRequest);

            //3. Get matching person object from List<Person> based on PersonID
            Person? matchingPerson = await _personsRepository.GetPersonByPersonID(personUpdateRequest.PersonID);

            //4. Check if matching person object is not null
            if (matchingPerson == null)
                throw new ArgumentException("Given PersonID doesn't exist");

            //5. Updates all details from PersonUpdateRequest object to Person object
            matchingPerson.PersonName = personUpdateRequest.PersonName;
            matchingPerson.Email = personUpdateRequest.Email;
            matchingPerson.DateOfBirth = personUpdateRequest.DateOfBirth;
            matchingPerson.Gender = personUpdateRequest.Gender.ToString();
            matchingPerson.CountryID = personUpdateRequest.CountryID;
            matchingPerson.Address = personUpdateRequest.Address;
            matchingPerson.ReceiveNewsLetters = personUpdateRequest.ReceiveNewsLetters;

            await _personsRepository.UpdatePerson(matchingPerson);


            //6. Convert the Person object to PersonResponse object
            //7. Return PersonResponse object with updated details
            //return ConvertPersonToPersonResponse(matchingPerson);
            return matchingPerson.ToPersonResponse();
        }

        public async Task<bool> DeletePerson(Guid? personID)
        {
            //1. Check if personID != null
            if (personID == null)
                throw new ArgumentNullException(nameof(personID));

            //2. Get matching person object from List<Person> based on personID
            Person? matchingPerson = await _personsRepository.GetPersonByPersonID(personID.Value);

            //3. Check if matching person object is not null
            if (matchingPerson == null)
                return false;

            //4. Delete matching person object from List<Person>
            await _personsRepository.DeletePersonByPersonID(personID.Value);

            //5. Return boolean value indicating person object was deleted or not
            return true;
        }

        public async Task<MemoryStream> GetPersonsCSV()
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

        public async Task<MemoryStream> GetPersonsExcel()
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