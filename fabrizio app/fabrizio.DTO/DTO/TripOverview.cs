using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fabrizio.Shared.DTO
{
	public class GETTripOverview
	{
		public TripDto? Previous { get; set; }
		public TripDto? Current { get; set; }
		public TripDto? Next { get; set; }
	}

}
