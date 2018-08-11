using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GenericCollections
{
    public class Set<T>: ISet<T>, IEnumerable<T>
    {
        #region Constants

        private const int DEFAULT_CAPACITY = 7;
        private readonly IEqualityComparer<T> comparer;

        #endregion

        #region Private fileds

        /// <summary>
        /// The buckets
        /// </summary>
        private Slot<T>[] buckets;

        #endregion

        #region .Ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="Set{T}"/> class.
        /// </summary>
        public Set() : this(DEFAULT_CAPACITY, EqualityComparer<T>.Default) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Set{T}"/> class.
        /// </summary>
        /// <param name="capacity">The capacity.</param>
        /// <param name="comparer">The comparer.</param>
        /// <exception cref="ArgumentNullException">comparer</exception>
        /// <exception cref="ArgumentException">capacity</exception>
        public Set(int capacity, EqualityComparer<T> comparer)
        {
            this.comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
            if (capacity <= 0)
            {
                throw new ArgumentException(nameof(capacity));
            }

            buckets = new Slot<T>[capacity];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Set{T}"/> class.
        /// </summary>
        /// <param name="values">The values.</param>
        public Set(IEnumerable<T> values) : this(values, EqualityComparer<T>.Default) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Set{T}"/> class.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="comparer">The comparer.</param>
        /// <exception cref="ArgumentNullException">
        /// comparer
        /// or
        /// values
        /// </exception>
        public Set(IEnumerable<T> values, EqualityComparer<T> comparer)
        {
            this.comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            buckets = new Slot<T>[GetNextPrime(values.Count())];
            foreach (var item in values)
            {
                Add(item);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        public int Count { get; private set; }

        /// <summary>
        /// Gets the capacity.
        /// </summary>
        /// <value>
        /// The capacity.
        /// </value>
        public int Capacity => buckets.Length;

        #endregion

        #region Public API

        /// <summary>
        /// Adds the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Add(T value)
        {
            SetCapacity();
            int bucketIndex = Math.Abs(value.GetHashCode() % buckets.Length);
            if (buckets[bucketIndex] == null)
            {
                buckets[bucketIndex] = new Slot<T>(value);
            }
            else
            {
                Slot<T> currSlot = buckets[bucketIndex];
                while (currSlot.Next != null)
                {
                    // In case of existion of new element in hash table
                    if (Contains(value))
                    {
                        return;
                    }

                    currSlot = currSlot.Next;
                }

                currSlot.Next = new Slot<T>(value);
            }

            Count++;
        }

        /// <summary>
        /// Determines whether [contains] [the specified value].
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <c>true</c> if [contains] [the specified value]; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(T value)
        {
            int bucketIndex = Math.Abs(value.GetHashCode() % buckets.Length);
            Slot<T> currBucket = buckets[bucketIndex];
            while (currBucket != null)
            {
                if (comparer.Equals(currBucket.Value, value))
                {
                    return true;
                }

                currBucket = currBucket.Next;
            }

            return false;
        }

        /// <summary>
        /// Removes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Remove(T value)
        {
            if (!this.Contains(value))
            {
                return;
            }

            int bucketIndex = Math.Abs(value.GetHashCode() % buckets.Length);

            if (comparer.Equals(this.buckets[bucketIndex].Value, value))
            {
                this.buckets[bucketIndex] = null;
                Count--;
                return;
            }

            Slot<T> parentSlot = buckets[bucketIndex];
            Slot<T> currentSlot = buckets[bucketIndex];
            while (currentSlot != null)
            {
                if (comparer.Equals(currentSlot.Value, value))
                {
                    parentSlot.Next = currentSlot.Next;
                    Count--;
                    return;
                }

                parentSlot = currentSlot;
                currentSlot = currentSlot.Next;
            }

            Count--;
        }

        /// <summary>
        /// Unions the with.
        /// </summary>
        /// <param name="anotherUnion">Another union.</param>
        /// <exception cref="ArgumentNullException">anotherUnion</exception>
        public void UnionWith(Set<T> anotherUnion)
        {
            if (anotherUnion == null)
            {
                throw new ArgumentNullException(nameof(anotherUnion));
            }

            foreach (var item in anotherUnion)
            {
                if (!this.Contains(item))
                {
                    this.Add(item);
                }
            }
        }

        /// <summary>
        /// Unions the specified LHS.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Set<T> Union(Set<T> lhs, Set<T> rhs)
        {
            if (lhs == null || rhs == null)
            {
                throw new ArgumentNullException($"One of the operands is equal to null");
            }

            foreach (var item in lhs)
            {
                if (!rhs.Contains(item))
                {
                    rhs.Add(item);
                }
            }

            return rhs;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            foreach (var item in buckets)
            {
                if (item != null)
                {
                    Slot<T> currSlot = item;
                    while (currSlot != null)
                    {
                        yield return currSlot.Value;
                        currSlot = currSlot.Next;
                    }
                }
            }

        }

        #endregion 

        #region Private fields

        private int GetNextPrime(int number)
        {
            while (true)
            {
                if (IsPrime(number))
                {
                    return number;
                }

                number++;
            }           
        }

        private static bool IsPrime(int number)
        {
            if (number == 1)
            {
                return false;
            }

            for (int i = 2; i <= (int)Math.Sqrt(number); i++)
            {
                if (number % i == 0)
                {
                    return false;
                }
            }

            return true;
        }

        private void SetCapacity()
        {
            if (Count >= buckets.Length)
            {
                int newSize = GetNextPrime(Count);
                Array.Resize(ref buckets, newSize);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion
    }
}
