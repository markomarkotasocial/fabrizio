namespace fabrizio.DTO
{
	public class GETTravelBooking
	{
		public Guid Id { get; set; }
		public int Type { get; set; }
		public string Origin { get; set; } = string.Empty;
		public string Destination { get; set; } = string.Empty;
		public string Reference { get; set; } = string.Empty;
		public string Carrier { get; set; } = string.Empty;
		public string Note { get; set; } = string.Empty;
		public DateTime? Departure { get; set; }
		public DateTime? Arrival { get; set; }
		public Guid? TripId { get; set; }
	}

	public class POSTTravelBooking
	{
		public int Type { get; set; }
		public string Origin { get; set; } = string.Empty;
		public string Destination { get; set; } = string.Empty;
		public string Reference { get; set; } = string.Empty;
		public string Carrier { get; set; } = string.Empty;
		public string Note { get; set; } = string.Empty;
		public DateTime? Departure { get; set; }
		public DateTime? Arrival { get; set; }

	}

	public class PUTTravelBooking
	{
		public Guid Id { get; set; }
		public int Type { get; set; }
		public string Origin { get; set; } = string.Empty;
		public string Destination { get; set; } = string.Empty;
		public string Reference { get; set; } = string.Empty;
		public string Carrier { get; set; } = string.Empty;
		public string Note { get; set; } = string.Empty;
		public DateTime? Departure { get; set; }
		public DateTime? Arrival { get; set; }
	}


}
