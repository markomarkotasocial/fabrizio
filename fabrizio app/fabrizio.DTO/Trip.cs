namespace fabrizio.DTO
{

	public class GETTripOverview
	{
		public GETTrip Previous { get; set; } 
		public GETTrip Current { get; set; } 
		public GETTrip Next { get; set; } 
	}

	public class GETTrip
	{
		public Guid Id { get; set; }
		public int Status { get; set; }
		public string Name { get; set; } = string.Empty;
		public string Notes { get; set; } = string.Empty;
		public DateTime? StartDate { get; set; }
		public DateTime? EndDate { get; set; }

		public IEnumerable<GETAccommodationBooking> AccommodationBookings { get; set; } = Enumerable.Empty<GETAccommodationBooking>();
		public IEnumerable<GETTravelBooking> TravelBookings { get; set; } = Enumerable.Empty<GETTravelBooking>();
		public IEnumerable<GETDestination> Destinations { get; set; } = Enumerable.Empty<GETDestination>();

	}

	public class POSTTrip
	{
		public string Name { get; set; } = string.Empty;
		public string Notes { get; set; } = string.Empty;
		public DateTime? StartDate { get; set; }
		public DateTime? EndDate { get; set; }
	}

	public class PUTTrip
	{
		public Guid Id { get; set; }
		public int Status { get; set; }
		public string Name { get; set; } = string.Empty;
		public string Notes { get; set; } = string.Empty;
		public DateTime? StartDate { get; set; }
		public DateTime? EndDate { get; set; }
	}


}
