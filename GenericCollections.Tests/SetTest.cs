using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace GenericCollections.Tests
{
    [TestFixture]
    class SetTest
    {
        [Test]
        public void TestCtor_SameElementsInCollection()
        {
            int[] expectedResult = { 55, 0, 3, 53, 6213, 6, 1, 656, -2, 55, 53, 656 };
            Set<int> setOfInt = new Set<int>(expectedResult);
            for (int i = 0; i < expectedResult.Length; i++)
            {
                Console.Write(expectedResult[i] + "    ");
            }
            Console.WriteLine();

            foreach (var item in setOfInt)
            {
                Console.Write(item + "    ");
            }
            Console.WriteLine();
        }

        [Test]
        public void TestContains()
        {
            int[] expectedResult = {55, 0, 3, 53, 6213, 6, 1, 656, -2};
            Set<int> setOfInt = new Set<int>(expectedResult);

            Assert.IsFalse(setOfInt.Contains(111));
            Assert.IsTrue(setOfInt.Contains(-2));
            Assert.IsTrue(setOfInt.Contains(55));
        }

        [Test]
        public void TestAdd_TwoElements()
        {
            var setOfInt = new Set<int>(new[]{1,2,3,4,5});
            setOfInt.Add(6);
            setOfInt.Add(-6);

            Assert.IsTrue(EqualUnions(setOfInt, new Set<int>{1,2,3,4,5,6,-6}));
        }

        [Test]
        public void TestUnionWith()
        {
            int[] firstArr = { 1, 2, 3, 4, 5 };
            int[] secondArr = { 3, 4, 5, 6, 7 };
            int[] expectedResult = {1, 2, 3, 4, 5, 6, 7};
            Set<int> firstSet = new Set<int>(firstArr);
            Set<int> secondSet = new Set<int>(secondArr);
            Set<int> resultSet = new Set<int>(expectedResult);
            firstSet.UnionWith(secondSet);
            foreach (var item in firstSet)
            {
                Console.Write(item + "    ");
            }
            Assert.IsTrue(EqualUnions(resultSet, firstSet));

        }

        [Test]
        public void TestStaticUnion_TwoSets()
        {
            int[] firstArr = { 1, 2, 3, 4, 5 };
            int[] secondArr = { 3, 4, 5, 6, 7 };
            Set<int> firstSet = new Set<int>(firstArr);
            Set<int> secondSet = new Set<int>(secondArr);
            Set<int> result = Set<int>.Union(firstSet, secondSet);
            foreach (var item in result)
            {
                Console.Write(item + "    ");
            }

            Assert.IsTrue(EqualUnions(result, new Set<int>(new[]{1,2,3,4,5,6,7})));
        }

        [Test]
        public void TestRemove_OneElement()
        {
            // simple O(1) removing
            var setOfInt = new Set<int>(new[] { 2, 3, 4, 5, 6, 7 });
            var resultSet = new Set<int>(new[] { 5, 6, 7 });
            setOfInt.Remove(2);
            setOfInt.Remove(3);
            setOfInt.Remove(4);

            Assert.IsTrue(EqualUnions(setOfInt, resultSet));
        }

        [Test]
        public void TestRemove_OneElementInChain()
        {
            // here will appear hash bucket with chain of slots 4 -> 14
            var setOfInt = new Set<int>(new[] { 2, 3, 4, 14 });
            var resultSet = new Set<int>(new [] { 2, 3, 4 });
            setOfInt.Remove(14);

            Assert.IsTrue(EqualUnions(setOfInt, resultSet));
        }

        /// <summary>
        /// Method for checking unions on equality
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rhs">The RHS.</param>
        /// <param name="lhs">The LHS.</param>
        /// <param name="comparer">The comparer.</param>
        /// <returns></returns>
        public static bool EqualUnions<T>(Set<T> rhs, Set<T> lhs,
            EqualityComparer<T> comparer = null)
        {
            if (rhs.Count != lhs.Count)
            {
                return false;
            }

            if (comparer == null)
            {
                comparer = EqualityComparer<T>.Default;
            }

            bool isFind = false;
            foreach (var rItem in rhs)
            {
                foreach (var lItem in lhs)
                {
                    if (comparer.Equals(lItem, rItem))
                    {
                        isFind = true;
                    }
                }

                if (!isFind)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
