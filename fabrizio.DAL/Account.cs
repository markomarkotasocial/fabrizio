using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fabrizio.DAL.Entities
{
	public enum AccountStatuses
	{
		Inactive = 0,
		Active,
		Suspended,
		Deleted
	}

	public class Account : BaseEntityInt
	{
		public AccountStatuses Status { get; set; } = AccountStatuses.Inactive;

		public required string Email { get; set; }		
		public string Name { get; set; } = string.Empty;

		public string? PasswordHash { get; set; }
		//public string? GoogleId { get; set; }


		public AccountInfo? AccountInfo { get; set; }

		public List<Trip> Trips { get; set; } = new();
		public List<AccommodationBooking> AccommodationBookings { get; set; } = new();
		public List<TravelBooking> TravelBookings { get; set; } = new();
		public List<Destination> Destinations { get; set; } = new();

	}

	public class AccountInfo : IAuditable
	{
		public int AccountId { get; set; }
		public Account Account { get; set; } = null!;

		public string PreferredLanguage { get; set; } = "en";
		public string PreferredCurrency { get; set; } = "EUR";
		public string TimeZone { get; set; } = "Europe/Zagreb";

		public bool IsDarkMode { get; set; } = false;


		public Guid? HomeLocationId { get; set; }
		public Location? HomeLocation { get; set; }


		public Audit Audit { get; set; } = new ();
	}

}


