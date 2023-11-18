using Microsoft.AspNetCore.Mvc.Filters;
using ServiceContracts.DTO;

namespace PersonManagement.Filters.ActionFilters
{
	public class PersonListActionFilter : IActionFilter
	{
		private readonly ILogger<PersonListActionFilter> _logger;

		public PersonListActionFilter(ILogger<PersonListActionFilter> logger)
		{
			_logger = logger;
		}

		public void OnActionExecuted(ActionExecutedContext context)
		{
			_logger.LogInformation(nameof(PersonListActionFilter) + $" {nameof(PersonListActionFilter.OnActionExecuted)} method");
		}

		public void OnActionExecuting(ActionExecutingContext context)
		{
			_logger.LogInformation(nameof(PersonListActionFilter) + $" {nameof(PersonListActionFilter.OnActionExecuting)} method");

			if(context.ActionArguments.ContainsKey("searchBy"))
			{
				string? searchBy = Convert.ToString(
					context.ActionArguments["searchBy"]
					);

				if (!string.IsNullOrEmpty(searchBy))
				{
					var searchByOptions = new List<string>()
					{
						nameof(PersonResponse.Name),
						nameof(PersonResponse.Address),
						nameof(PersonResponse.Email),
						nameof(PersonResponse.DateOfBirth),
						nameof(PersonResponse.Gender)
					};
					if (searchByOptions.Any(temp => temp == searchBy))
					{
						_logger.LogInformation("Search By actual value {searchBy}", searchBy);
						context.ActionArguments["searchBy"] = nameof(PersonResponse.Name);
						_logger.LogInformation("Search By updated value {searchBy}", searchBy);
					}
				}
			}
		}
	}
}
