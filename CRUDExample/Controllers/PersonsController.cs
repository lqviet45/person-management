using Entities;
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
                { nameof(Person.Name), "Person Name" },
                { nameof(Person.Email), "Email" },
                { nameof(Person.DateOfBirth), "Date Of Birth" },
                { nameof(Person.Gender), "Gender" },
                { nameof(Person.CountryID), "Country" }
            };
            List<PersonResponse> personList = _personServices.GetAllPersons();

            return View(personList);
        }
    }
}
