using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fabrizio.App.Resources.Lookups
{
	public class LanguageOption
	{
		public string Code { get; set; } = default!;
		public string Name { get; set; } = default!;
	}

	public static class LanguageData
	{
		// ISO 639-1
		public static readonly List<LanguageOption> All = new()
		{
			new() { Code = "en", Name = "English" },
			new() { Code = "es", Name = "Spanish" },
			new() { Code = "fr", Name = "French" },
			new() { Code = "de", Name = "German" },
			new() { Code = "it", Name = "Italian" },
			new() { Code = "pt", Name = "Portuguese" },
			new() { Code = "nl", Name = "Dutch" },
			new() { Code = "hr", Name = "Croatian" },
			new() { Code = "sr", Name = "Serbian" },
			new() { Code = "bs", Name = "Bosnian" },
			new() { Code = "sl", Name = "Slovenian" },
			new() { Code = "hu", Name = "Hungarian" },
			new() { Code = "pl", Name = "Polish" },
			new() { Code = "cs", Name = "Czech" },
			new() { Code = "sk", Name = "Slovak" },
			new() { Code = "ro", Name = "Romanian" },
			new() { Code = "bg", Name = "Bulgarian" },
			new() { Code = "el", Name = "Greek" },
			new() { Code = "tr", Name = "Turkish" },
			new() { Code = "ru", Name = "Russian" },
			new() { Code = "uk", Name = "Ukrainian" },
			new() { Code = "ar", Name = "Arabic" },
			new() { Code = "he", Name = "Hebrew" },
			new() { Code = "hi", Name = "Hindi" },
			new() { Code = "th", Name = "Thai" },
			new() { Code = "vi", Name = "Vietnamese" },
			new() { Code = "id", Name = "Indonesian" },
			new() { Code = "ms", Name = "Malay" },
			new() { Code = "zh", Name = "Chinese" },
			new() { Code = "ja", Name = "Japanese" },
			new() { Code = "ko", Name = "Korean" }
		};
	}
}
