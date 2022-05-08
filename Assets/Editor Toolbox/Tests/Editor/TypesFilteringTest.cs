using System;

using NUnit.Framework;

using UnityEngine;

namespace Toolbox.Editor.Tests
{
    using Toolbox.Editor.Internal;

    public class TypesFilteringTest
    {
        public interface Interface1 { }
        public interface Interface2 : Interface1 { }
        public interface Interface3 : Interface1 { }
        public interface Interface4 : Interface2 { }
        public interface Interface4<T> : Interface3 { }

        public abstract class ClassBase : Interface1 { }
        public class ClassWithInterface1 : ClassBase { }
        [Obsolete]
        public class ClassWithInterface2 : ClassBase { }
        public class ClassWithInterface3 : ClassBase { }


        [TestCase(typeof(ClassBase), 3)]
        [TestCase(typeof(Interface1), 6)]
        [TestCase(typeof(MonoBehaviour), 2)]
        public void TestTypesCachingPass(Type parentType, int count)
        {
            TypeUtilities.ClearCache();
            for (var i = 0; i < count; i++)
            {
                TypeUtilities.GetCollection(parentType);
            }

            Assert.AreEqual(1, TypeUtilities.cachedCollections.Count);
        }

        [Test]
        public void TestStandardConstraintPass1()
        {
            var constraint = new TypeConstraintStandard(typeof(Interface1), TypeSettings.Class, false, false);
            var collection = TypeUtilities.GetCollection(constraint);
            Assert.IsFalse(collection.Contains(typeof(ClassBase)));
            Assert.IsTrue(collection.Contains(typeof(ClassWithInterface1)));
#pragma warning disable CS0612
            Assert.IsFalse(collection.Contains(typeof(ClassWithInterface2)));
#pragma warning restore CS0612
            Assert.IsTrue(collection.Contains(typeof(ClassWithInterface3)));
            Assert.IsFalse(collection.Contains(typeof(Interface2)));
            Assert.IsFalse(collection.Contains(typeof(Interface3)));
        }

        [Test]
        public void TestStandardConstraintPass2()
        {
            var constraint = new TypeConstraintStandard(typeof(Interface1), TypeSettings.Class, true, false);
            var collection = TypeUtilities.GetCollection(constraint);
            Assert.IsTrue(collection.Contains(typeof(ClassBase)));
            Assert.IsTrue(collection.Contains(typeof(ClassWithInterface1)));
#pragma warning disable CS0612
            Assert.IsFalse(collection.Contains(typeof(ClassWithInterface2)));
#pragma warning restore CS0612
            Assert.IsTrue(collection.Contains(typeof(ClassWithInterface3)));
            Assert.IsFalse(collection.Contains(typeof(Interface2)));
            Assert.IsFalse(collection.Contains(typeof(Interface3)));
        }

        [Test]
        public void TestStandardConstraintPass3()
        {
            var constraint = new TypeConstraintStandard(typeof(Interface1), TypeSettings.Class, true, true);
            var collection = TypeUtilities.GetCollection(constraint);
            Assert.IsTrue(collection.Contains(typeof(ClassBase)));
            Assert.IsTrue(collection.Contains(typeof(ClassWithInterface1)));
#pragma warning disable CS0612
            Assert.IsTrue(collection.Contains(typeof(ClassWithInterface2)));
#pragma warning restore CS0612
            Assert.IsTrue(collection.Contains(typeof(ClassWithInterface3)));
            Assert.IsFalse(collection.Contains(typeof(Interface2)));
            Assert.IsFalse(collection.Contains(typeof(Interface3)));
        }

        [Test]
        public void TestStandardConstraintPass4()
        {
            var constraint = new TypeConstraintStandard(typeof(Interface1), TypeSettings.Interface, true, true);
            var collection = TypeUtilities.GetCollection(constraint);
            Assert.IsFalse(collection.Contains(typeof(ClassBase)));
            Assert.IsFalse(collection.Contains(typeof(ClassWithInterface1)));
#pragma warning disable CS0612
            Assert.IsFalse(collection.Contains(typeof(ClassWithInterface2)));
#pragma warning restore CS0612
            Assert.IsFalse(collection.Contains(typeof(ClassWithInterface3)));
            Assert.IsTrue(collection.Contains(typeof(Interface2)));
            Assert.IsTrue(collection.Contains(typeof(Interface3)));
        }

        [Test]
        public void TestStandardConstraintPass5()
        {
            var constraint = new TypeConstraintStandard(typeof(Interface1), TypeSettings.Class | TypeSettings.Interface, true, false);
            var collection = TypeUtilities.GetCollection(constraint);
            Assert.IsTrue(collection.Contains(typeof(ClassBase)));
            Assert.IsTrue(collection.Contains(typeof(ClassWithInterface1)));
#pragma warning disable CS0612
            Assert.IsFalse(collection.Contains(typeof(ClassWithInterface2)));
#pragma warning restore CS0612
            Assert.IsTrue(collection.Contains(typeof(ClassWithInterface3)));
            Assert.IsTrue(collection.Contains(typeof(Interface2)));
            Assert.IsTrue(collection.Contains(typeof(Interface3)));
        }

        [Test]
        public void TestStandardConstraintPass6()
        {
            var constraint = new TypeConstraintStandard(typeof(ClassBase), TypeSettings.Class | TypeSettings.Interface, false, false);
            var collection = TypeUtilities.GetCollection(constraint);
            Assert.IsFalse(collection.Contains(typeof(ClassBase)));
            Assert.IsTrue(collection.Contains(typeof(ClassWithInterface1)));
#pragma warning disable CS0612
            Assert.IsFalse(collection.Contains(typeof(ClassWithInterface2)));
#pragma warning restore CS0612
            Assert.IsTrue(collection.Contains(typeof(ClassWithInterface3)));
            Assert.IsFalse(collection.Contains(typeof(Interface2)));
            Assert.IsFalse(collection.Contains(typeof(Interface3)));
        }

        [Test]
        public void TestStandardConstraintPass7()
        {
            var constraint = new TypeConstraintStandard(typeof(Component), TypeSettings.Class | TypeSettings.Interface, true, false);
            var collection = TypeUtilities.GetCollection(constraint);
            Assert.IsFalse(collection.Contains(typeof(ClassBase)));
            Assert.IsTrue(collection.Contains(typeof(Collider)));
        }

        [Test]
        public void TestReferenceConstraintPass1()
        {
            var constraint = new TypeConstraintReference(typeof(Component));
            var collection = TypeUtilities.GetCollection(constraint);
            Assert.AreEqual(0, collection.Values.Count);
        }

        [Test]
        public void TestReferenceConstraintPass2()
        {
            var constraint = new TypeConstraintReference(typeof(ClassBase));
            var collection = TypeUtilities.GetCollection(constraint);
            Assert.IsTrue(collection.Contains(typeof(ClassWithInterface1)));
#pragma warning disable CS0612
            Assert.IsFalse(collection.Contains(typeof(ClassWithInterface2)));
#pragma warning restore CS0612
            Assert.IsTrue(collection.Contains(typeof(ClassWithInterface3)));
            Assert.IsFalse(collection.Contains(typeof(Interface2)));
            Assert.IsFalse(collection.Contains(typeof(Interface3)));
        }

        [Test]
        public void TestReferenceConstraintPass3()
        {
            var constraint = new TypeConstraintReference(typeof(Interface1));
            var collection = TypeUtilities.GetCollection(constraint);
            Assert.IsTrue(collection.Contains(typeof(ClassWithInterface1)));
#pragma warning disable CS0612
            Assert.IsFalse(collection.Contains(typeof(ClassWithInterface2)));
#pragma warning restore CS0612
            Assert.IsTrue(collection.Contains(typeof(ClassWithInterface3)));
            Assert.IsFalse(collection.Contains(typeof(Interface2)));
            Assert.IsFalse(collection.Contains(typeof(Interface3)));
        }
    }
}