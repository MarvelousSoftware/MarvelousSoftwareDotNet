namespace MarvelousSoftware.Grid.DataSource
{
    public interface IRequestWrapper
    {
        string this[string name] { get; }
        bool Has(string name);
    }
}