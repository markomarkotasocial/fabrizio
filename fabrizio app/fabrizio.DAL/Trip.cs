using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fabrizio.DAL.Entities
{
	public enum TripStatus
	{
		Planned,
		Ongoing,
		Completed,
		Cancelled
	}

	public class Trip : BaseEntityGuid
	{
		public TripStatus Status { get; set; } = TripStatus.Planned;

		public string Name { get; set; } = string.Empty;
		public string Destination { get; set; } = string.Empty;
		public DateTime? StartDate { get; set; }
		public DateTime? EndDate { get; set; }

		public List<AccomodationBooking> AccomodationBookings { get; set; } = new();
		public List<TravelBooking> TravelBookings { get; set; } = new();

		public int AccountId { get; set; }
		public Account? Account { get; set; }

	}
}
