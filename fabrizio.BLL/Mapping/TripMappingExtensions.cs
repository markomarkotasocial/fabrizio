using fabrizio.DAL.Entities;
using fabrizio.Shared.DTO;

namespace fabrizio.BLL
{
	/// <summary>
	/// Entity → DTO projections for <see cref="Trip"/> and its child aggregates.
	/// Single source of truth for these mappings, previously duplicated inline
	/// across the <c>TripService</c> partial files.
	/// </summary>
	public static class TripMappingExtensions
	{
		public static DestinationDto ToDto(this Destination d) => new DestinationDto
		{
			Id = d.Id,
			Order = d.Order,
			Name = d.Name,
			TripId = d.TripId,
		};

		public static TravelBookingDto ToDto(this TravelBooking tb) => new TravelBookingDto
		{
			Id = tb.Id,
			Type = (int)tb.Type,
			Origin = tb.Origin,
			Destination = tb.Destination,
			Reference = tb.Reference,
			Carrier = tb.Carrier,
			Note = tb.Note,
			Departure = tb.Departure,
			Arrival = tb.Arrival,
			TripId = tb.TripId,
		};

		public static AccommodationBookingDto ToDto(this AccommodationBooking ab) => new AccommodationBookingDto
		{
			Id = ab.Id,
			Type = (int)ab.Type,
			Location = ab.Location,
			Name = ab.Name,
			Reference = ab.Reference,
			Note = ab.Note,
			From = ab.From,
			To = ab.To,
			TripId = ab.TripId,
		};

		public static TripDto ToDto(this Trip trip) => new TripDto
		{
			Id = trip.Id,
			Status = (int)trip.Status,
			Name = trip.Name,
			Notes = trip.Notes ?? string.Empty,
			StartDate = trip.StartDate,
			EndDate = trip.EndDate,
			Destinations = (trip.Destinations ?? Enumerable.Empty<Destination>())
				.OrderBy(d => d.Order).Select(d => d.ToDto()).ToList(),
			TravelBookings = (trip.TravelBookings ?? Enumerable.Empty<TravelBooking>())
				.Select(tb => tb.ToDto()).ToList(),
			AccommodationBookings = (trip.AccommodationBookings ?? Enumerable.Empty<AccommodationBooking>())
				.Select(ab => ab.ToDto()).ToList(),
		};

		public static TripListItemDto ToListItemDto(this Trip trip) => new TripListItemDto
		{
			Id = trip.Id,
			Status = (int)trip.Status,
			Name = trip.Name,
			Notes = trip.Notes ?? string.Empty,
			StartDate = trip.StartDate,
			EndDate = trip.EndDate,
			Destinations = (trip.Destinations ?? Enumerable.Empty<Destination>())
				.OrderBy(d => d.Order).Select(d => d.ToDto()).ToList(),
		};
	}
}
