using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fabrizio.App.Resources.Lookups
{
	public class CurrencyOption
	{
		public string Code { get; init; } = default!;   // EUR
		public string Name { get; init; } = default!;   // Euro
		public string Symbol { get; init; } = default!; // €

		// runtime flag
		public bool IsSelected { get; set; }
	}

	public static class CurrencyData
	{
		public static readonly List<CurrencyOption> All = new List<CurrencyOption>
		{
			new() { Code = "EUR", Name = "Euro", Symbol = "€" },
			new() { Code = "USD", Name = "US Dollar", Symbol = "$" },
			new() { Code = "GBP", Name = "British Pound", Symbol = "£" },		
			new() { Code = "CHF", Name = "Swiss Franc", Symbol = "CHF" },
			new() { Code = "JPY", Name = "Japanese Yen", Symbol = "¥" },
			new() { Code = "CNY", Name = "Chinese Yuan", Symbol = "¥" },
			new() { Code = "AUD", Name = "Australian Dollar", Symbol = "$" },
			new() { Code = "CAD", Name = "Canadian Dollar", Symbol = "$" },
			new() { Code = "NZD", Name = "New Zealand Dollar", Symbol = "$" },
			new() { Code = "SEK", Name = "Swedish Krona", Symbol = "kr" },
			new() { Code = "NOK", Name = "Norwegian Krone", Symbol = "kr" },
			new() { Code = "DKK", Name = "Danish Krone", Symbol = "kr" },
			new() { Code = "PLN", Name = "Polish Zloty", Symbol = "zł" },
			new() { Code = "CZK", Name = "Czech Koruna", Symbol = "Kč" },
			new() { Code = "THB", Name = "Thai Baht", Symbol = "฿" }
		};
	}
}
