using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fabrizio.App.Resources.Lookups
{
	public class TimeZoneOption
	{
		public string Id { get; set; } = default!;
		public string DisplayName { get; set; } = default!;

		// runtime flag
		public bool IsSelected { get; set; }
	}

	public static class TimeZoneData
	{
		public static readonly List<TimeZoneOption> All = new()
		{
			new() { Id = "UTC", DisplayName = "UTC" },
			new() { Id = "Europe/London", DisplayName = "(UTC+0/+1) London" },
			new() { Id = "Europe/Berlin", DisplayName = "(UTC+1/+2) Berlin" },
			new() { Id = "Europe/Zagreb", DisplayName = "(UTC+1/+2) Zagreb" },
			new() { Id = "Europe/Paris", DisplayName = "(UTC+1/+2) Paris" },
			new() { Id = "America/New_York", DisplayName = "(UTC-5/-4) New York" },
			new() { Id = "America/Chicago", DisplayName = "(UTC-6/-5) Chicago" },
			new() { Id = "America/Denver", DisplayName = "(UTC-7/-6) Denver" },
			new() { Id = "America/Los_Angeles", DisplayName = "(UTC-8/-7) Los Angeles" },
			new() { Id = "Asia/Dubai", DisplayName = "(UTC+4) Dubai" },
			new() { Id = "Asia/Bangkok", DisplayName = "(UTC+7) Bangkok" },
			new() { Id = "Asia/Singapore", DisplayName = "(UTC+8) Singapore" },
			new() { Id = "Asia/Tokyo", DisplayName = "(UTC+9) Tokyo" },
			new() { Id = "Australia/Sydney", DisplayName = "(UTC+10/+11) Sydney" } 
		};

	}
}
