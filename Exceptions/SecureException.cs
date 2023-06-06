namespace TreeNodes.Exceptions
{
    public class SecureException : Exception
    {
        public string EventId { get; }

        public SecureException(string eventId, string message) : base(message)
        {
            EventId = eventId;
        }
    }
}
