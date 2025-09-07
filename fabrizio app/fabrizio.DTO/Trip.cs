namespace fabrizio.DTO
{
	public class GETTrip
	{
		public Guid Id { get; set; }
		public int Status { get; set; }
		public string Name { get; set; } = string.Empty;
		public string Destination { get; set; } = string.Empty;
		public DateTime? StartDate { get; set; }
		public DateTime? EndDate { get; set; }

	}

	public class POSTTrip
	{
		public string Name { get; set; } = string.Empty;
		public string Destination { get; set; } = string.Empty;
		public DateTime? StartDate { get; set; }
		public DateTime? EndDate { get; set; }
	}

	public class PUTTrip
	{
		public Guid Id { get; set; }
		public int Status { get; set; }
		public string Name { get; set; } = string.Empty;
		public string Destination { get; set; } = string.Empty;
		public DateTime? StartDate { get; set; }
		public DateTime? EndDate { get; set; }
	}


}
