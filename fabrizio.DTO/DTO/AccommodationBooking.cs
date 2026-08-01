using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fabrizio.Shared.DTO
{
	public class AccommodationBookingDto
	{
		public Guid Id { get; set; }
		public int Type { get; set; }
		public string Location { get; set; } = string.Empty;
		public string Name { get; set; } = string.Empty;
		public string Reference { get; set; } = string.Empty;
		public string Note { get; set; } = string.Empty;
		public DateTime? From { get; set; }
		public DateTime? To { get; set; }
		public Guid? TripId { get; set; }

	}

	public class CreateAccommodationBookingRequest
	{
		public int Type { get; set; }
		public string Location { get; set; } = string.Empty;
		public string Name { get; set; } = string.Empty;
		public string Reference { get; set; } = string.Empty;
		public string Note { get; set; } = string.Empty;
		public DateTime? From { get; set; }
		public DateTime? To { get; set; }
	}

	public class UpdateAccommodationBookingRequest
	{
		public Guid Id { get; set; }
		public int Type { get; set; }
		public string Location { get; set; } = string.Empty;
		public string Name { get; set; } = string.Empty;
		public string Reference { get; set; } = string.Empty;
		public string Note { get; set; } = string.Empty;
		public DateTime? From { get; set; }
		public DateTime? To { get; set; }
	}


}
