using Microsoft.AspNetCore.Mvc;
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
        public IActionResult Index(string searchBy, string? searchString,
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

            List<PersonResponse> personList = _personServices.GetFilteredPersons(searchBy, searchString);
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
        public IActionResult Create()
        {
            var countriesList = _countriesService.GetAllCountries();
            ViewBag.Countries = countriesList;

            return View();
        }

        [HttpPost]
        [Route("create")]
        public IActionResult Create(PersonAddRequest personAddRequest)
        {
            if (!ModelState.IsValid)
            {
                var countriesList = _countriesService.GetAllCountries();
                ViewBag.Countries = countriesList;

                ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors)
                                    .Select(e => e.ErrorMessage).ToList();
                return View();
            }

            var personResponse = _personServices.AddPerson(personAddRequest);
            return RedirectToAction("Index", "Persons");
        }
    }
}
