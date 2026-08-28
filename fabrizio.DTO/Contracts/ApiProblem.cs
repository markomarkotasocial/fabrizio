using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fabrizio.Shared.Contracts
{
	public class ApiProblem
	{
		public string? Type { get; set; }
		public string? Title { get; set; }
		public string? Detail { get; set; }
		public int? Status { get; set; }

		// Populated for ASP.NET ValidationProblemDetails (model-binding failures).
		public Dictionary<string, string[]>? Errors { get; set; }
	}

}
