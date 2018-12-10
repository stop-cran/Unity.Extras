using NUnit.Framework;
using Shouldly;
using Unity;
using Unity.Exceptions;
using Unity.Injection;
using Unity.Resolution;
using UnityExtras.DefaultValueResolver;

namespace UnityExtras.DefaultParameterValue.Tests
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void ShouldAddExtension() =>
            new UnityContainer().AddNewExtension<DefaultParameterValueExtension>();

        [Test]
        public void ShouldNotResolveConstructorDefaultsWithoutExtension() =>
            Should.Throw<ResolutionFailedException>(() =>
               new UnityContainer().Resolve<ClassA>());

        [Test]
        public void ShouldResolveAllConstructorDefaultsViaExtension()
        {
            var a = new UnityContainer()
                .AddNewExtension<DefaultParameterValueExtension>()
                .Resolve<ClassA>();

            a.X.ShouldBe(34);
            a.S.ShouldBe("qwerty");
            a.Obj.ShouldBeNull();
        }

        [Test]
        public void ExtensionShouldNotInterferRegistrations()
        {
            var obj = new object();
            var a = new UnityContainer()
                .AddNewExtension<DefaultParameterValueExtension>()
                .RegisterType<object>()
                 .RegisterType<ClassA>(
                    new InjectionConstructor(
                        new ResolvedParameter<int>(),
                        new ResolvedParameter<string>(),
                        obj))
                .Resolve<ClassA>();

            a.X.ShouldBe(34);
            a.S.ShouldBe("qwerty");
            a.Obj.ShouldBeSameAs(obj);
        }

        [Test]
        public void ShouldResolveAllConstructorDefaultsViaInjectionParameterValue()
        {
            var a = new UnityContainer()
                .RegisterType<ClassA>(
                    new InjectionConstructor(
                        new DefaultParameterValue<int>(),
                        new DefaultParameterValue<string>(),
                        new DefaultParameterValue<object>()))
                .Resolve<ClassA>();

            a.X.ShouldBe(34);
            a.S.ShouldBe("qwerty");
            a.Obj.ShouldBeNull();
        }

        [Test]
        public void ShouldNotInterferExplicitParameterValue()
        {
            var a = new UnityContainer()
                .RegisterType<ClassA>(
                    new InjectionConstructor(
                        47,
                        new DefaultParameterValue<string>(),
                        new DefaultParameterValue<object>()))
                .Resolve<ClassA>();

            a.X.ShouldBe(47);
            a.S.ShouldBe("qwerty");
            a.Obj.ShouldBeNull();
        }

        [Test]
        public void ShouldNotResolveParameterValue() =>
            Should.Throw<ResolutionFailedException>(() =>
                new UnityContainer()
                    .RegisterType<ClassA>(
                        new InjectionConstructor(
                            new ResolvedParameter<int>(),
                            new DefaultParameterValue<string>(),
                            new DefaultParameterValue<object>()))
                    .Resolve<ClassA>());

        [Test]
        public void ShouldNotResolveDefaultParameterValueForNonOptionalParameter() =>
         Should.Throw<ResolutionFailedException>(() =>
             new UnityContainer()
                 .RegisterType<ClassB>(
                     new InjectionConstructor(
                         new ResolvedParameter<int>(),
                         new DefaultParameterValue<string>()))
                 .Resolve<ClassB>());

        [Test]
        public void ShouldResolveDefaultParameterValueTogetherWithExplicit()
        {
            var b = new UnityContainer()
                .RegisterType<ClassB>(
                    new InjectionConstructor(
                        47,
                        new DefaultParameterValue<string>()))
                .Resolve<ClassB>();

            b.X.ShouldBe(47);
            b.S.ShouldBe("qwerty");
        }

        [Test]
        public void ShouldResolveWithExtensionAndOverrides()
        {
            var b = new UnityContainer()
                .AddNewExtension<DefaultParameterValueExtension>()
                .Resolve<ClassB>(new ParameterOverride("x", 45));

            b.X.ShouldBe(45);
            b.S.ShouldBe("qwerty");
        }

        [Test]
        public void ShouldResolveParentWithExtensionAndParameterOverride()
        {
            var c = new UnityContainer()
                .AddNewExtension<DefaultParameterValueExtension>()
                .Resolve<ClassC>(new ParameterOverride("x", 45));

            c.B.X.ShouldBe(45);
            c.B.S.ShouldBe("qwerty");
        }

        [Test]
        public void ShouldResolveParentWithExtensionAndDependencyOverride()
        {
            var c = new UnityContainer()
                .AddNewExtension<DefaultParameterValueExtension>()
                .Resolve<ClassC>(new DependencyOverride<int>(45));

            c.B.X.ShouldBe(45);
            c.B.S.ShouldBe("qwerty");
        }

        [Test]
        public void ShouldResolveParentDefaultParameterValueTogetherWithExplicit()
        {
            var c = new UnityContainer()
                .RegisterType<ClassB>(
                    new InjectionConstructor(
                        47,
                        new DefaultParameterValue<string>()))
                .Resolve<ClassC>();

            c.B.X.ShouldBe(47);
            c.B.S.ShouldBe("qwerty");
        }

        [Test]
        public void ShouldResolveMethodParameter()
        {
            var d = new UnityContainer()
               .RegisterType<ClassD>(
                new InjectionMethod("SetX",
                new DefaultParameterValue<int>()))
                .Resolve<ClassD>();

            d.X.ShouldBe(26);
        }

        [Test]
        public void ShouldResolveMethodParameterViaExtension()
        {
            var d = new UnityContainer()
                .AddNewExtension<DefaultParameterValueExtension>()
                .RegisterType<ClassD>(
                    new InjectionMethod("SetX",
                        new ResolvedParameter<int>()))
                    .Resolve<ClassD>();

            d.X.ShouldBe(26);
        }

        private sealed class ClassA
        {
            public ClassA(int x = 34, string s = "qwerty", object obj = null)
            {
                X = x;
                S = s;
                Obj = obj;
            }

            public int X { get; }
            public string S { get; }
            public object Obj { get; }
        }

        private sealed class ClassB
        {
            public ClassB(int x, string s = "qwerty")
            {
                X = x;
                S = s;
            }

            public int X { get; }
            public string S { get; }
        }

        private sealed class ClassC
        {
            public ClassC(ClassB b)
            {
                B = b;
            }

            public ClassB B { get; }
        }

        private sealed class ClassD
        {
            public int X { get; private set; }

            public void SetX(int x = 26) => X = x;
        }
    }
}