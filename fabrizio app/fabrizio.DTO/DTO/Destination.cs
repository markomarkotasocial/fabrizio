using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fabrizio.Shared.DTO
{
	public class DestinationDto
	{
		public Guid Id { get; set; }
		public int Order { get; set; }
		public string Name { get; set; } = string.Empty;
		public Guid? TripId { get; set; }
	}

	public class CreateDestinationRequest
	{
		public string Name { get; set; } = string.Empty;
	}

	public class UpdateDestinationRequest
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = string.Empty;
	}


}
