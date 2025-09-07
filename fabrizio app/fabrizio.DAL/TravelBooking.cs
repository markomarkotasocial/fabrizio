using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fabrizio.DAL.Entities
{
	public enum TravelBookingTypes
	{
		Other = 0,
		Train,
		Bus,
		Airplane,
		Car,
		Ferry,
		SpeedBoat,		
	}

	public class TravelBooking : BaseEntityGuid
	{
		public TravelBookingTypes Type { get; set; }

		public string Origin { get; set; } = string.Empty;
		//public string OriginGpsCoordinates { get; set; } = string.Empty;
		public string Destination { get; set; } = string.Empty;		
		//public string DestinationGpsCoordinates { get; set; } = string.Empty;   // npr airport gps coordinates	
		public string Reference { get; set; } = string.Empty;
		public string Carrier { get; set; } = string.Empty;
		public string Note { get; set; } = string.Empty;

		public DateTime? Departure { get; set; }
		public DateTime? Arrival { get; set; }

		public Guid? TripId { get; set; }
		public virtual Trip? Trip { get; set; }

		public int AccountId { get; set; }
		public Account? Account { get; set; }


	}
}
