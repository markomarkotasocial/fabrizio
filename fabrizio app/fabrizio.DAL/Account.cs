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
		public string PasswordHash { get; set; } = string.Empty; // store securely
		public string Name { get; set; } = string.Empty;


		public List<Trip> Trips { get; set; } = new();
		public List<AccomodationBooking> AccomodationBookings { get; set; } = new();
		public List<TravelBooking> TravelBookings { get; set; } = new();


	}

}


