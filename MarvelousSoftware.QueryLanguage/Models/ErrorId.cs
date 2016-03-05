namespace MarvelousSoftware.QueryLanguage.Models
{
    /// <summary>
    /// Identification of specific error. Provides basic information about error.
    /// </summary>
    public enum ErrorId
    {
        /// <summary>
        /// Occurres if column name provided in the query has been not found.
        /// </summary>
        ColumnNotFound,

        /// <summary>
        /// Occurres if literal is not parsable.
        /// </summary>
        InvalidLiteral,

        /// <summary>
        /// Occurres if given compare operator has been not found.
        /// </summary>
        CompareOperatorNotFound,

        /// <summary>
        /// Occurres if given logical operator has been not found.
        /// </summary>
        LogicalOperatorNotFound,

        /// <summary>
        /// Occures if close paren found without prior open paren.
        /// </summary>
        ParenCloseWithoutParenOpen,

        /// <summary>
        /// Occures if paren close has been not found.
        /// </summary>
        ParenCloseNotFound,

        /// <summary>
        /// Occures if paren open has been not found.
        /// </summary>
        ParenOpenNotFound,

        /// <summary>
        /// Occures if statement has been not found.
        /// </summary>
        StatementNotFound,

        /// <summary>
        /// Occures if column doesn't support used compare operator.
        /// </summary>
        NotSupportedCompareOperator,

        /// <summary>
        /// Occures if parser detects that right operand of the logical operator is missing.
        /// </summary>
        LogicalOperatorRightOperandMissing,

        /// <summary>
        /// Occures if query is incomlete and it's impossible to create even a single expression.
        /// </summary>
        QueryWithoutEvenSingleExpression,

        /// <summary>
        /// Occures if function has invalid name (e.g. contains whitespaces).
        /// </summary>
        InvalidFunctionName,

        /// <summary>
        /// Occures if function has parameter open identifier, but is not closed.
        /// </summary>
        FunctionParamsCloseMissing,
        
        /// <summary>
        /// Occures if function syntax of a function is correct, but the function itself does not exist.
        /// </summary>
        FunctionNotDefined,

        /// <summary>
        /// Occures if function returns a type which is incompatible with current column type.
        /// </summary>
        FunctionReturnsIncompatibleType,

        /// <summary>
        /// Occures if lexer cannot handle some part of the query and unknown tokens are not allowed.
        /// </summary>
        UnknownExpression,

        /// <summary>
        /// Occures only in case of bugs.
        /// </summary>
        UnexpectedError
    }
}
