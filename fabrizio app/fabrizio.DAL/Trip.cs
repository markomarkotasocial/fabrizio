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

		public virtual List<AccommodationBooking> AccommodationBookings { get; set; } = new();
		public virtual List<TravelBooking> TravelBookings { get; set; } = new();

		public int AccountId { get; set; }
		public Account? Account { get; set; }


		public void Recalculate()
		{
			if (AccommodationBookings != null && AccommodationBookings.Any())
			{
				var minStart = AccommodationBookings.Where(x => x.From.HasValue).Min(x => x.From);
				var maxEnd = AccommodationBookings.Where(x => x.To.HasValue).Max(x => x.To);
				if (minStart.HasValue)
				{
					if (!StartDate.HasValue || minStart.Value < StartDate.Value)
					{
						StartDate = minStart.Value;
					}
				}
				if (maxEnd.HasValue)
				{
					if (!EndDate.HasValue || maxEnd.Value > EndDate.Value)
					{
						EndDate = maxEnd.Value;
					}
				}
			}
			if (TravelBookings != null && TravelBookings.Any())
			{
				var minDeparture = TravelBookings.Where(x => x.Departure.HasValue).Min(x => x.Departure);
				var maxArrival = TravelBookings.Where(x => x.Arrival.HasValue).Max(x => x.Arrival);
				if (minDeparture.HasValue)
				{
					if (!StartDate.HasValue || minDeparture.Value < StartDate.Value)
					{
						StartDate = minDeparture.Value;
					}
				}
				if (maxArrival.HasValue)
				{
					if (!EndDate.HasValue || maxArrival.Value > EndDate.Value)
					{
						EndDate = maxArrival.Value;
					}
				}
			}
		}

	}
}
