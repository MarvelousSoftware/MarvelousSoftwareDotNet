using System.Collections.Generic;
using System.Linq;

namespace MarvelousSoftware.QueryLanguage.Models
{
    public class ErrorList : List<Error>
    {
        public ErrorList Add(string message, ErrorId id, ErrorType type)
        {
            Add(new Error(message, id, type));
            return this;
        }

        /// <summary>
        /// Combines multiple errors into just one, readable even for non-technical people.
        /// </summary>
        public bool TryGetSingleError(out string error)
        {
            if (Count == 1)
            {
                error = this[0].Message;
                return true;
            }

            if (Count == 2 
                && this.Any(x => x.Id == ErrorId.StatementNotFound) 
                && this.Any(x => x.Id == ErrorId.CompareOperatorNotFound)
                && this[0].Text == this[1].Text)
            {
                error = $"\"{this[0].Text}\" is neither statement nor compare operator.";
                return true;
            }

            // these are rather not helpful to non-technical people
            RemoveAll(x => x.Id == ErrorId.ParenOpenNotFound);
            RemoveAll(x => x.Id == ErrorId.ParenCloseNotFound);

            // TODO: translations?
            error = this.FirstOrDefault(x => x.Id == ErrorId.ColumnNotFound)?.Message
                        ?? this.FirstOrDefault(x => x.Id == ErrorId.NotSupportedCompareOperator)?.Message
                        ?? this.FirstOrDefault(x => x.Id == ErrorId.ParenCloseWithoutParenOpen)?.Message;

            return error != null;
        }

        public string[] GetSingleMessageIfPossible()
        {
            string error;
            if (TryGetSingleError(out error))
            {
                return new[] {error};
            }

            return this.Select(x => x.Message).ToArray();
        }
    }
}
