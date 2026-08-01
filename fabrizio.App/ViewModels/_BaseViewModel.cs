using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fabrizio.App.ViewModels
{
	public abstract partial class BaseViewModel : ObservableObject
	{
		[ObservableProperty]
		private bool isBusy;

		[ObservableProperty]
		private string? emptyMessage;

		[ObservableProperty]
		private bool hasError;
	}
}
