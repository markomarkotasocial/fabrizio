using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fabrizio.DAL.Entities
{
	public enum AccommodationBookingTypes
	{
		Other = 0,
		Hotel,
		Hostel,
		Private,		
	}

	public class AccommodationBooking : BaseEntityGuid
	{

		public AccommodationBookingTypes Type { get; set; }

		public string Location { get; set; } = string.Empty;
		//public string GpsCoordinates { get; set; } = string.Empty;
		public string Name { get; set; } = string.Empty;
		public string Reference { get; set; } = string.Empty;
		public string Note { get; set; } = string.Empty;

		public DateTime? From { get; set; }
		public DateTime? To { get; set; }

		public Guid? TripId { get; set; }
		public virtual Trip? Trip { get; set; }

		public int AccountId { get; set; }
		public virtual Account? Account { get; set; }

	}
}
