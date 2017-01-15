namespace Be.Runtime.Types
{
    public class NamespaceCollection : ListCollection<NamespaceSymbol>
    {  }

    public class NamespaceSymbol
    {
        public string Path;
        public ObjectCollection Objects = new ObjectCollection();

        public NamespaceSymbol(string Path)
        {
            this.Path = Path;
        }
    }
}
