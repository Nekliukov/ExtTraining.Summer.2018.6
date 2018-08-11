namespace GenericCollections
{
    internal interface ISet<T>
    {
        void Add(T value);
        void Remove(T value);
        bool Contains(T value);
        void UnionWith(Set<T> value);
    }
}
