using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fabrizio.DAL.Entities
{
	public class Location : BaseEntityGuid
	{
		public string CountryCode { get; set; } = null!; // HR
		public string? City { get; set; }               // Zagreb
		public double? Latitude { get; set; }
		public double? Longitude { get; set; }

	}
}
