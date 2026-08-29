using System;
using System.Collections.Generic;
using System.Linq;

using fabrizio.Shared.DTO;

namespace fabrizio.App.ViewModels
{
	/// <summary>
	/// Presentation wrapper around <see cref="TripListItemDto"/> for the trips list.
	/// The computed display members used to live on the shared DTO; they belong on
	/// the client (device-local clock, no reason to travel over the wire).
	/// </summary>
	public class TripCardModel
	{
		private readonly TripListItemDto _dto;

		public TripCardModel(TripListItemDto dto) => _dto = dto;

		public Guid Id => _dto.Id;
		public int Status => _dto.Status;
		public string Name => _dto.Name;
		public string Notes => _dto.Notes;
		public DateTime? StartDate => _dto.StartDate;
		public DateTime? EndDate => _dto.EndDate;
		public IEnumerable<DestinationDto> Destinations => _dto.Destinations;

		public bool IsCurrent =>
			StartDate.HasValue && EndDate.HasValue &&
			DateTime.Today >= StartDate.Value.Date && DateTime.Today <= EndDate.Value.Date;

		public bool ShowCountdown => StartDate.HasValue && StartDate.Value.Date > DateTime.Today;

		public int DaysLeft => StartDate.HasValue ? Math.Max(0, (StartDate.Value.Date - DateTime.Today).Days) : 0;

		public IEnumerable<DestinationDto> VisibleDestinations => Destinations?.Take(2) ?? Enumerable.Empty<DestinationDto>();

		public int HiddenDestinationsCount => Destinations == null ? 0 : Math.Max(0, Destinations.Count() - 2);

		public bool HasHiddenDestinations => HiddenDestinationsCount > 0;

		public string DateRangeText =>
			(StartDate.HasValue && EndDate.HasValue)
				? ((StartDate.Value.Year != DateTime.UtcNow.Year || EndDate.Value.Year != DateTime.UtcNow.Year)
					? $"{StartDate:dd MMM yyyy} — {EndDate:dd MMM yyyy}"
					: $"{StartDate:dd MMM} — {EndDate:dd MMM}")
				: string.Empty;
	}
}
