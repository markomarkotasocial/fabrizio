namespace fabrizio.DTO
{
	public class GETAccommodationBooking
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

	public class POSTAccommodationBooking
	{
		public int Type { get; set; }
		public string Location { get; set; } = string.Empty;
		public string Name { get; set; } = string.Empty;
		public string Reference { get; set; } = string.Empty;
		public string Note { get; set; } = string.Empty;
		public DateTime? From { get; set; }
		public DateTime? To { get; set; }
		public Guid? TripId { get; set; }
	}

	public class PUTAccommodationBooking
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


}
