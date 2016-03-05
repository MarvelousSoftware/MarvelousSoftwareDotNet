using System.Collections.Generic;

namespace MarvelousSoftware.QueryLanguage.Lexer.Functions
{
    public interface IFunctionDefinitions
    {
        IEnumerable<FunctionDefinition> Definitions { get; }
        FunctionDefinition Get(string name);
        bool Has(string name);
        void Define(FunctionDefinition function);
        void Override(FunctionDefinition function);
    }
}