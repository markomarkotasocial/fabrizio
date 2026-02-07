using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fabrizio.Shared.DTO
{
	public class GETDestination
	{
		public Guid Id { get; set; }
		public int Order { get; set; }
		public string Name { get; set; } = string.Empty;
		public Guid? TripId { get; set; }
	}

	public class POSTDestination
	{
		public string Name { get; set; } = string.Empty;

	}

	public class PUTDestination
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = string.Empty;
	}


}
