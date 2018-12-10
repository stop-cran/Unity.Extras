# Overview [![NuGet](https://img.shields.io/nuget/v/UnityExtras.DefaultParameterValue.svg)](https://www.nuget.org/packages/UnityExtras.DefaultParameterValue) [![Build Status](https://travis-ci.com/stop-cran/Unity.Extras.DefaultParameterValue.svg?branch=master)](https://travis-ci.com/stop-cran/Unity.Extras.DefaultParameterValue)

This package provides an extension for [Unity Container](https://github.com/unitycontainer/unity) that resolves constructor and method default values taking the values via reflection.

# Installation

NuGet package is available [here](https://www.nuget.org/packages/UnityExtras.DefaultParameterValue/).

```PowerShell
PM> Install-Package UnityExtras.DefaultParameterValue
```

# Example

Suppose there's a class with several optional constructor parameters:

```C#
class ClassA
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
```

Suppose then, we're going to use the default values when building an instance of this class.
Such an intension can be specified per parameter:

```C#
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
```

The second option is to use a container extension to resolve **all** parameter default values, unless explicitly specified:

```C#
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
```
