#pragma warning disable CS0612

using System;
using NUnit.Framework;
using UnityEngine;

namespace Toolbox.Editor.Tests
{
    using Toolbox.Editor.Internal.Types;

    public class TypesFilteringTest
    {
        public interface Interface1 { }
        [Obsolete]
        public interface Interface2 : Interface1 { }
        public interface Interface3 : Interface1 { }
        public interface Interface4 : Interface2 { }
        public interface Interface4<T> : Interface3 { }
        public interface Interface5 : Interface4<int> { }
        public interface Interface6 : Interface4<string> { }
        public interface Interface7<T> : Interface4<T> { }
        public interface Interface8<T1, T2> : Interface4<T1> { }
        public interface Interface9 : Interface8<int, int> { }

        public abstract class ClassBase : Interface1 { }

        public class ClassWithInterface1 : ClassBase { }
        [Obsolete]
        public class ClassWithInterface2 : ClassBase { }
        public class ClassWithInterface3 : ClassBase { }
        public class ClassWithInterface4<T> : ClassBase, Interface4<T> { }
        public class ClassWithInterface5 : ClassWithInterface4<int> { }
        public class ClassWithInterface6 : ClassWithInterface4<string> { }
        public class ClassWithInterface7<T> : ClassWithInterface4<T> { }
        public class ClassWithInterface8<T1, T2> : ClassWithInterface4<T1> { }
        public class ClassWithInterface9 : ClassWithInterface8<int, int> { }

        [TestCase(typeof(ClassBase), 3)]
        [TestCase(typeof(Interface1), 6)]
        [TestCase(typeof(MonoBehaviour), 2)]
        public void TestTypesCachingPass(Type parentType, int count)
        {
            TypeUtility.ClearCache();
            for (var i = 0; i < count; i++)
            {
                TypeUtility.GetCollection(parentType);
            }

            Assert.AreEqual(1, TypeUtility.cachedCollections.Count);
        }

        [Test]
        public void TestStandardConstraintPass1()
        {
            var constraint = new TypeConstraintStandard(typeof(Interface1), TypeSettings.Class, false, false);
            var collection = TypeUtility.GetCollection(constraint);
            Assert.IsFalse(collection.Contains(typeof(ClassBase)));
            Assert.IsTrue(collection.Contains(typeof(ClassWithInterface1)));
            Assert.IsFalse(collection.Contains(typeof(ClassWithInterface2)));
            Assert.IsTrue(collection.Contains(typeof(ClassWithInterface3)));
            Assert.IsFalse(collection.Contains(typeof(Interface2)));
            Assert.IsFalse(collection.Contains(typeof(Interface3)));
        }

        [Test]
        public void TestStandardConstraintPass2()
        {
            var constraint = new TypeConstraintStandard(typeof(Interface1), TypeSettings.Class, true, false);
            var collection = TypeUtility.GetCollection(constraint);
            Assert.IsTrue(collection.Contains(typeof(ClassBase)));
            Assert.IsTrue(collection.Contains(typeof(ClassWithInterface1)));
            Assert.IsFalse(collection.Contains(typeof(ClassWithInterface2)));
            Assert.IsTrue(collection.Contains(typeof(ClassWithInterface3)));
            Assert.IsFalse(collection.Contains(typeof(Interface2)));
            Assert.IsFalse(collection.Contains(typeof(Interface3)));
        }

        [Test]
        public void TestStandardConstraintPass3()
        {
            var constraint = new TypeConstraintStandard(typeof(Interface1), TypeSettings.Class, true, true);
            var collection = TypeUtility.GetCollection(constraint);
            Assert.IsTrue(collection.Contains(typeof(ClassBase)));
            Assert.IsTrue(collection.Contains(typeof(ClassWithInterface1)));
            Assert.IsTrue(collection.Contains(typeof(ClassWithInterface2)));
            Assert.IsTrue(collection.Contains(typeof(ClassWithInterface3)));
            Assert.IsFalse(collection.Contains(typeof(Interface2)));
            Assert.IsFalse(collection.Contains(typeof(Interface3)));
        }

        [Test]
        public void TestStandardConstraintPass4()
        {
            var constraint = new TypeConstraintStandard(typeof(Interface1), TypeSettings.Interface, true, true);
            var collection = TypeUtility.GetCollection(constraint);
            Assert.IsFalse(collection.Contains(typeof(ClassBase)));
            Assert.IsFalse(collection.Contains(typeof(ClassWithInterface1)));
            Assert.IsFalse(collection.Contains(typeof(ClassWithInterface2)));
            Assert.IsFalse(collection.Contains(typeof(ClassWithInterface3)));
            Assert.IsTrue(collection.Contains(typeof(Interface2)));
            Assert.IsTrue(collection.Contains(typeof(Interface3)));
        }

        [Test]
        public void TestStandardConstraintPass5()
        {
            var constraint = new TypeConstraintStandard(typeof(Interface1), TypeSettings.Class | TypeSettings.Interface, true, false);
            var collection = TypeUtility.GetCollection(constraint);
            Assert.IsTrue(collection.Contains(typeof(ClassBase)));
            Assert.IsTrue(collection.Contains(typeof(ClassWithInterface1)));
            Assert.IsFalse(collection.Contains(typeof(ClassWithInterface2)));
            Assert.IsTrue(collection.Contains(typeof(ClassWithInterface3)));
            Assert.IsFalse(collection.Contains(typeof(Interface2)));
            Assert.IsTrue(collection.Contains(typeof(Interface3)));
        }

        [Test]
        public void TestStandardConstraintPass6()
        {
            var constraint = new TypeConstraintStandard(typeof(ClassBase), TypeSettings.Class | TypeSettings.Interface, false, false);
            var collection = TypeUtility.GetCollection(constraint);
            Assert.IsFalse(collection.Contains(typeof(ClassBase)));
            Assert.IsTrue(collection.Contains(typeof(ClassWithInterface1)));
            Assert.IsFalse(collection.Contains(typeof(ClassWithInterface2)));
            Assert.IsTrue(collection.Contains(typeof(ClassWithInterface3)));
            Assert.IsFalse(collection.Contains(typeof(Interface2)));
            Assert.IsFalse(collection.Contains(typeof(Interface3)));
        }

        [Test]
        public void TestStandardConstraintPass7()
        {
            var constraint = new TypeConstraintStandard(typeof(Component), TypeSettings.Class | TypeSettings.Interface, true, false);
            var collection = TypeUtility.GetCollection(constraint);
            Assert.IsFalse(collection.Contains(typeof(ClassBase)));
            Assert.IsTrue(collection.Contains(typeof(Collider)));
        }

        [Test]
        public void TestStandardConstraintWithGenericPass()
        {
            var constraint = new TypeConstraintStandard(typeof(Interface4<int>), TypeSettings.Interface, true, false);
            var collection = TypeUtility.GetCollection(constraint);
            Assert.IsFalse(collection.Contains(typeof(ClassWithInterface1)));
            Assert.IsFalse(collection.Contains(typeof(ClassWithInterface2)));
            Assert.IsFalse(collection.Contains(typeof(ClassWithInterface3)));
            Assert.IsFalse(collection.Contains(typeof(Interface2)));
            Assert.IsFalse(collection.Contains(typeof(Interface3)));
            Assert.IsTrue(collection.Contains(typeof(Interface4<int>)));
            Assert.IsTrue(collection.Contains(typeof(Interface5)));
            Assert.IsFalse(collection.Contains(typeof(Interface6)));
            Assert.IsTrue(collection.Contains(typeof(Interface7<int>)));
            Assert.IsFalse(collection.Contains(typeof(Interface8<string, string>)));
            Assert.IsTrue(collection.Contains(typeof(Interface9)));

            //NOTE: not supported since the 2nd argument should be "picked", we don't want to generate all available options
            Assert.IsFalse(collection.Contains(typeof(Interface8<int, int>)));
            Assert.IsFalse(collection.Contains(typeof(Interface8<int, string>)));
        }

        [Test]
        public void TestSerializeReferenceConstraintPass1()
        {
            var constraint = new TypeConstraintSerializeReference(typeof(Component));
            var collection = TypeUtility.GetCollection(constraint);
            Assert.AreEqual(0, collection.Values.Count);
        }

        [Test]
        public void TestSerializeReferenceConstraintPass2()
        {
            var constraint = new TypeConstraintSerializeReference(typeof(ClassBase));
            var collection = TypeUtility.GetCollection(constraint);
            Assert.IsTrue(collection.Contains(typeof(ClassWithInterface1)));
            Assert.IsFalse(collection.Contains(typeof(ClassWithInterface2)));
            Assert.IsTrue(collection.Contains(typeof(ClassWithInterface3)));
            Assert.IsFalse(collection.Contains(typeof(Interface2)));
            Assert.IsFalse(collection.Contains(typeof(Interface3)));
        }

        [Test]
        public void TestSerializeReferenceConstraintPass3()
        {
            var constraint = new TypeConstraintSerializeReference(typeof(Interface1));
            var collection = TypeUtility.GetCollection(constraint);
            Assert.IsTrue(collection.Contains(typeof(ClassWithInterface1)));
            Assert.IsFalse(collection.Contains(typeof(ClassWithInterface2)));
            Assert.IsTrue(collection.Contains(typeof(ClassWithInterface3)));
            Assert.IsFalse(collection.Contains(typeof(Interface2)));
            Assert.IsFalse(collection.Contains(typeof(Interface3)));
        }

        [Test]
        public void TestSerializeReferenceConstraintWithGenericPass()
        {
            var constraint = new TypeConstraintSerializeReference(typeof(Interface4<int>));
            var collection = TypeUtility.GetCollection(constraint);
            Assert.IsFalse(collection.Contains(typeof(ClassWithInterface1)));
            Assert.IsFalse(collection.Contains(typeof(ClassWithInterface2)));
            Assert.IsFalse(collection.Contains(typeof(ClassWithInterface3)));
            Assert.IsFalse(collection.Contains(typeof(Interface2)));
            Assert.IsFalse(collection.Contains(typeof(Interface3)));
            Assert.IsFalse(collection.Contains(typeof(Interface5)));
            Assert.IsFalse(collection.Contains(typeof(Interface6)));
            Assert.IsFalse(collection.Contains(typeof(Interface7<int>)));
            Assert.IsFalse(collection.Contains(typeof(ClassWithInterface4<string>)));
            Assert.IsTrue(collection.Contains(typeof(ClassWithInterface5)));
            Assert.IsFalse(collection.Contains(typeof(ClassWithInterface6)));
            Assert.IsFalse(collection.Contains(typeof(ClassWithInterface7<string>)));
            Assert.IsTrue(collection.Contains(typeof(ClassWithInterface9)));
#if UNITY_2023_2_OR_NEWER
            Assert.IsTrue(collection.Contains(typeof(ClassWithInterface4<int>)));
            Assert.IsTrue(collection.Contains(typeof(ClassWithInterface7<int>)));
#else
            Assert.IsFalse(collection.Contains(typeof(ClassWithInterface4<int>)));
            Assert.IsFalse(collection.Contains(typeof(ClassWithInterface7<int>)));
#endif

            //NOTE: not supported since the 2nd argument should be "picked", we don't want to generate all available options
            Assert.IsFalse(collection.Contains(typeof(ClassWithInterface8<int, int>)));
        }
    }
}

#pragma warning restore CS0612