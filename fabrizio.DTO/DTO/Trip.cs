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


		#region UI Getters

		public bool IsCurrent => StartDate.HasValue && EndDate.HasValue && DateTime.Today >= StartDate.Value.Date && DateTime.Today <= EndDate.Value.Date;

		public bool ShowCountdown => StartDate.HasValue && StartDate.Value.Date > DateTime.Today;

		public int DaysLeft => StartDate.HasValue ? Math.Max(0, (StartDate.Value.Date - DateTime.Today).Days) : 0;

		public IEnumerable<DestinationDto> VisibleDestinations => Destinations?.Take(2) ?? Enumerable.Empty<DestinationDto>();

		public int HiddenDestinationsCount => Destinations == null ? 0 : Math.Max(0, Destinations.Count() - 2);

		public bool HasHiddenDestinations => HiddenDestinationsCount > 0;

		public string DateRangeText => (StartDate.HasValue && EndDate.HasValue) ? ((StartDate.Value.Year != DateTime.UtcNow.Year || EndDate.Value.Year != DateTime.UtcNow.Year) ? $"{StartDate:dd MMM yyyy} — {EndDate:dd MMM yyyy}" : $"{StartDate:dd MMM} — {EndDate:dd MMM}") : string.Empty;


		#endregion UI Getters


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
