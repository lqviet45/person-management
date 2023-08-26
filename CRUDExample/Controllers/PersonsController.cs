using Microsoft.AspNetCore.Mvc;
using ServiceContracts;
using ServiceContracts.DTO;

namespace CRUDExample.Controllers
{
    public class PersonsController : Controller
    {

        private readonly IPersonServices _personServices;
        public PersonsController(IPersonServices personService) 
        {
            _personServices = personService;
        }

        [Route("persons/index")]
        [Route("/")]
        public IActionResult Index(string searchBy, string? searchString)
        {
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

            return View(personList);
        }
    }
}
