namespace MarvelousSoftware.Core
{
    public interface IRequestWrapper
    {
        string this[string name] { get; }
        bool Has(string name);
    }
}