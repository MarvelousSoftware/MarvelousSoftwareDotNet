namespace MarvelousSoftware.QueryLanguage.Models
{
    /// <summary>
    /// Contains error related info.
    /// </summary>
    public class Error
    {
        public Error(string message, ErrorId id, ErrorType type)
            : this(message, id, type, null)
        {
        }

        public Error(string message, ErrorId id, ErrorType type, string text)
        {
            Message = message;
            Type = type;
            Text = text;
            Id = id;
        }

        public string Message { get; private set; }
        public ErrorType Type { get; private set; }
        public ErrorId Id { get; private set; }
        public string Text { get; private set; }
    }
}
