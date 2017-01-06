namespace Be.Runtime.Types
{
    public class UsingCollection : ListCollection<UsingSymbol>
    { }

    public class UsingSymbol
    {
        public string Path;

        public UsingSymbol(string Path)
        {
            this.Path = Path;
        }
    }
}
