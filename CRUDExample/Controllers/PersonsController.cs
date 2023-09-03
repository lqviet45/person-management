using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace CRUDExample.Controllers
{
    [Route("persons")]
    public class PersonsController : Controller
    {

        private readonly IPersonServices _personServices;
        private readonly ICountriesService _countriesService;
        public PersonsController(IPersonServices personService, ICountriesService countriesService) 
        {
            _personServices = personService;
            _countriesService = countriesService;
        }

        [Route("index")]
        [Route("/")]
        public async Task<IActionResult> Index(string searchBy, string? searchString,
            string sortBy = nameof(PersonResponse.Name), SortOrderOptions sortOrder = SortOrderOptions.ASC)
        {
            //Searching
            ViewBag.SearchFields = new Dictionary<string, string>()
            {
                { nameof(PersonResponse.Name), "Person Name" },
                { nameof(PersonResponse.Email), "Email" },
                { nameof(PersonResponse.DateOfBirth), "Date Of Birth" },
                { nameof(PersonResponse.Gender), "Gender" },
                { nameof(PersonResponse.CountryID), "Country" }
            };

            List<PersonResponse> personList = await _personServices.GetFilteredPersons(searchBy, searchString);
            ViewBag.SearchBy = searchBy;
            ViewBag.SearchString = searchString;

            //Sort
            var sortedPersons = _personServices.GetSortedPersons(personList, sortBy, sortOrder);
            ViewBag.SortBy = sortBy;
            ViewBag.SortOrder = sortOrder.ToString();

            return View(sortedPersons);
        }

        [Route("create")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var countriesList = await _countriesService.GetAllCountries();
            ViewBag.Countries = countriesList
                .Select(country => new SelectListItem()
                {
                    Text = country.CountryName,
                    Value = country.CountryID.ToString()
                });

            
            return View();
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Create(PersonAddRequest personAddRequest)
        {
            if (!ModelState.IsValid)
            {
                var countriesList = await _countriesService.GetAllCountries();
                ViewBag.Countries = countriesList
                    .Select(country => new SelectListItem()
                    {
                        Text = country.CountryName,
                        Value = country.CountryID.ToString()
                    });

                ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors)
                                    .Select(e => e.ErrorMessage).ToList();
                return View();
            }

            PersonResponse personResponse = 
                await _personServices.AddPerson(personAddRequest);
            return RedirectToAction("Index", "Persons");
        }


        [HttpGet]
        [Route("edit/{personID}")]
        public async Task<IActionResult> Edit(Guid personID)
        {
            PersonResponse? personUpdate = await _personServices.GetPersonByID(personID);

            if (personUpdate == null)
            {
                return RedirectToAction("Index", "Persons");
            }

            PersonUpdateRequest personUpdateRequest = personUpdate.ToPersonUpdateRequest();

            var countriesList = await _countriesService.GetAllCountries();
            ViewBag.Countries = countriesList
                .Select(country => new SelectListItem()
                {
                    Text = country.CountryName,
                    Value = country.CountryID.ToString()
                });

            return View(personUpdateRequest);
        }


        [HttpPost]
        [Route("edit/{personID}")]
        public async Task<IActionResult> Edit(PersonUpdateRequest personUpdateRequest)
        {
            var personUpdate = await _personServices.GetPersonByID(personUpdateRequest.PersonID);
            if (personUpdate == null)
            {
                return RedirectToAction("Index", "Persons");
            }
            if (ModelState.IsValid)
            {
                var personUpdated = await _personServices.UpdetePerson(personUpdateRequest);
                return RedirectToAction("Index", "Persons");
            }
            else
            {
                var countriesList = await _countriesService.GetAllCountries();
                ViewBag.Countries = countriesList
                    .Select(country => new SelectListItem()
                    {
                        Text = country.CountryName,
                        Value = country.CountryID.ToString()
                    });

                ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors)
                                    .Select(e => e.ErrorMessage).ToList();
                return View(personUpdate.ToPersonUpdateRequest());
            }
        }

        [HttpGet]
        [Route("delete/{personID}")]
        public async Task<IActionResult> Delete(Guid? personID)
        {
            PersonResponse? personDelete = await _personServices.GetPersonByID(personID);
            if (personDelete == null)
            {
                return RedirectToAction("Index", "Persons");
            }
            return View(personDelete);
        }

        [HttpPost]
        [Route("delete/{personID}")]
        public async Task<IActionResult> Delete(PersonUpdateRequest personUpdateRequest)
        {
            PersonResponse? person = await _personServices.GetPersonByID(personUpdateRequest.PersonID);
            if (person == null)
            {
                return RedirectToAction("Index", "Persons");
            }
            await _personServices.DeletePerson(personUpdateRequest.PersonID);
            return RedirectToAction("Index", "Persons");
        }
    }
}
