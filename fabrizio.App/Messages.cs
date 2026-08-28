namespace fabrizio.App
{
	/// <summary>
	/// Sent after a trip is created, edited or deleted so any open trip list
	/// (currently <c>TripsViewModel</c>) reloads the next time it appears.
	/// </summary>
	public sealed record TripsChangedMessage;
}
