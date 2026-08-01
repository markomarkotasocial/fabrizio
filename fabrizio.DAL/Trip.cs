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


	/// <summary>
	/// Represents a travel plan containing destinations and bookings.
	/// Trip dates represent the overall time span of the journey
	/// and may be derived from booking data.
	/// </summary>
	public class Trip : BaseEntityGuid
	{
		/// <summary>
		/// Trip lifecycle status.
		/// Planned/Ongoing/Completed are derived from StartDate and EndDate.
		/// Cancelled is a terminal state set explicitly by the user and
		/// prevents further modifications.
		/// </summary>
		public TripStatus Status { get; set; } = TripStatus.Planned;

		public string Name { get; set; } = string.Empty;
		public string? Notes { get; set; }

		/// <summary>
		/// Earliest date of the trip.
		/// May be expanded automatically when bookings are added.
		/// Cannot be removed if bookings exist (validated in BLL).
		/// </summary>
		public DateTime? StartDate { get; set; }

		/// <summary>
		/// Earliest date of the trip.
		/// May be expanded automatically when bookings are added.
		/// Cannot be removed if bookings exist (validated in BLL).
		/// </summary>
		public DateTime? EndDate { get; set; }

		public virtual List<AccommodationBooking> AccommodationBookings { get; set; } = new();
		public virtual List<TravelBooking> TravelBookings { get; set; } = new();
		public virtual List<Destination> Destinations { get; set; } = new();

		public int AccountId { get; set; }
		public Account? Account { get; set; }



		/// <summary>
		/// Latest date of the trip.
		/// May be expanded automatically when bookings are added.
		/// Cannot be removed if bookings exist (validated in BLL).
		/// </summary>
		public void Recalculate()
		{
			DateTime? minStart = null;
			DateTime? maxEnd = null;

			// Accommodation bookings
			foreach (var booking in AccommodationBookings)
			{
				if (booking.From.HasValue)
				{
					if (!minStart.HasValue || booking.From.Value < minStart.Value)
						minStart = booking.From.Value;
				}

				if (booking.To.HasValue)
				{
					if (!maxEnd.HasValue || booking.To.Value > maxEnd.Value)
						maxEnd = booking.To.Value;
				}
			}

			// Travel bookings
			foreach (var booking in TravelBookings)
			{
				if (booking.Departure.HasValue)
				{
					if (!minStart.HasValue || booking.Departure.Value < minStart.Value)
						minStart = booking.Departure.Value;
				}

				if (booking.Arrival.HasValue)
				{
					if (!maxEnd.HasValue || booking.Arrival.Value > maxEnd.Value)
						maxEnd = booking.Arrival.Value;
				}
			}

			// Apply results
			if (minStart.HasValue)
			{
				if (!StartDate.HasValue || minStart.Value < StartDate.Value)
					StartDate = minStart.Value;
			}

			if (maxEnd.HasValue)
			{
				if (!EndDate.HasValue || maxEnd.Value > EndDate.Value)
					EndDate = maxEnd.Value;
			}

			RecalculateStatus();
		}

		private void RecalculateStatus()
		{
			if (Status == TripStatus.Cancelled) return;

			var today = DateTime.UtcNow.Date;

			if (StartDate.HasValue && StartDate.Value.Date > today)
			{
				Status = TripStatus.Planned;
				return;
			}

			if (StartDate.HasValue && StartDate.Value.Date <= today)
			{
				if (!EndDate.HasValue || EndDate.Value.Date >= today)
				{
					Status = TripStatus.Ongoing;
					return;
				}
			}

			if (EndDate.HasValue && EndDate.Value.Date < today)
			{
				Status = TripStatus.Completed;
			}
		}

		public void Cancel()
		{
			Status = TripStatus.Cancelled;
		}



	}
}
