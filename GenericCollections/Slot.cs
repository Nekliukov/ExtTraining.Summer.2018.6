namespace GenericCollections
{
    internal class Slot<T>
    {
        internal T Value { get; set; }

        internal Slot<T> Next { get; set; }

        internal Slot(T Value)
        {
            this.Value = Value;
        }

    }
}
