using System.Collections.Generic;

namespace MarvelousSoftware.QueryLanguage.Lexing.Functions
{
    public class FunctionDefinitions : IFunctionDefinitions
    {
        private readonly IDictionary<string, FunctionDefinition> _definitions = new Dictionary<string, FunctionDefinition>();

        public IEnumerable<FunctionDefinition> Definitions => _definitions.Values;

        public FunctionDefinition Get(string name)
        {
            if (Has(name) == false)
            {
                return null;
            }

            return _definitions[name];
        }

        public bool Has(string name)
        {
            return _definitions.ContainsKey(name);
        }

        public void Define(FunctionDefinition function)
        {
            if (_definitions.ContainsKey(function.Name))
            {
                throw new QueryLanguageException($"Function '{function.Name}' is already defined. Use Override if you would like to change the behaviour of existing function.");
            }

            Override(function);
        }

        public void Override(FunctionDefinition function)
        {
            _definitions[function.Name] = function;
        }
    }
}