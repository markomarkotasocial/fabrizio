using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fabrizio.Shared.DTO
{
	public enum TripFilter
	{
		All = 0,
		Past = 1,
		CurrentAndUpcoming = 2,
		Wishlist = 3
	}
	public class TripDto
	{
		public Guid Id { get; set; }
		public int Status { get; set; }
		public string Name { get; set; } = string.Empty;
		public string Notes { get; set; } = string.Empty;
		public DateTime? StartDate { get; set; }
		public DateTime? EndDate { get; set; }

		public IEnumerable<AccommodationBookingDto> AccommodationBookings { get; set; } = Enumerable.Empty<AccommodationBookingDto>();
		public IEnumerable<TravelBookingDto> TravelBookings { get; set; } = Enumerable.Empty<TravelBookingDto>();
		public IEnumerable<DestinationDto> Destinations { get; set; } = Enumerable.Empty<DestinationDto>();


	}

	public class TripListItemDto
	{
		public Guid Id { get; set; }
		public int Status { get; set; }
		public string Name { get; set; } = string.Empty;
		public string Notes { get; set; } = string.Empty;
		public DateTime? StartDate { get; set; }
		public DateTime? EndDate { get; set; }


		public IEnumerable<DestinationDto> Destinations { get; set; } = Enumerable.Empty<DestinationDto>();
	}

	public class CreateTripRequest
	{
		public string Name { get; set; } = string.Empty;
		public string Notes { get; set; } = string.Empty;
		public DateTime? StartDate { get; set; }
		public DateTime? EndDate { get; set; }
	}

	public class UpdateTripRequest
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public string Notes { get; set; } = string.Empty;
		public DateTime? StartDate { get; set; }
		public DateTime? EndDate { get; set; }
	}


}
