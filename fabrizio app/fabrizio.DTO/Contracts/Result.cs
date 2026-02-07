using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fabrizio.Shared.Contracts
{
	public record BusinessError(
		string Code,
		string Message,
		int HttpStatusCode
		);

	
	public class Result
	{
		public Result() { }

		public bool IsSuccess { get; }
		public BusinessError? Error { get; }

		protected Result(bool success, BusinessError? error)
		{
			IsSuccess = success;
			Error = error;
		}

		public static Result Success() => new(true, null);
		public static Result Fail(BusinessError error) => new(false, error);
	}

	public class Result<T> : Result
	{
		public T? Value { get; set; }
		public Result() { }

		private Result(bool success, T? value, BusinessError? error) : base(success, error)
		{
			Value = value;
		}

		public static Result<T> Success(T value) => new(true, value, null);
		public static Result<T> Fail(BusinessError error) => new(false, default, error);
	}
}
