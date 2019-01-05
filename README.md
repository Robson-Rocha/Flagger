# Flagger

![Hit Count](http://hits.dwyl.io/robson-rocha/flagger.svg)
![Travis Build Status](https://travis-ci.org/Robson-Rocha/Flagger.svg?branch=master)
![AppVeyor Build Status](https://ci.appveyor.com/api/projects/status/i14mh50vtke5xrxw?svg=true)

**Flagger** is a small library which help you to set and unset (or toggle) a variable, field or property at the begin and end of an **using** scope. It is very useful for WPF properties, when you need to set a Dependency Property -- e.g. "IsWorking" -- while some background processing is being done to show and hide a panel using binding to a visibility property, or even to implement lightweight locks.

The purpose is to help switching values for a short period. It is tailored not only for ``bool`` values (the most obvious usage), but for any type.

As **Flagger** is based on the [IDisposable](https://docs.microsoft.com/dotnet/api/system.idisposable) interface, it can be used with the ``using`` instruction, and thus is exception-safe (who never ran into problems by not unsetting a flag?).

**Flagger** also provide methods to deal with recursive flag setting codes, allowing to setting and unsetting a flag only if it was not previously set, avoiding a recursive or nested method to inadvertently unset a flag prematurely.

## How to use Flagger?

Before anything else, [import most current version of the NuGet package](https://www.nuget.org/packages/Flagger) to your project. It targets for both .NET Standard 1.0 and .NET Standard 2.0, so it can be used in any .NET Core project and in .NET Framework 4.0 or greater, [including many other .NET Framework versions](https://docs.microsoft.com/dotnet/standard/net-standard).

The most simple way to use **Flagger** is with the [Flag](#flag) static class methods. See these examples to learn more!

### 1. Toggling boolean values with *[Flag.Toggle](#flagtogglemember)*

Use [Flag.Toggle](#flagtogglemember) when you need to invert (toggle) momentarily the value of a ``bool`` variable, field or property.

```csharp
using Flagger;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Flagger.Samples.Toggle {

  class Program {
    
    //Boolean property to be toggled
    static bool IsProcessing { get; set; } = false;

    //Long working method which sets the property during operation to signal the UI
    static async Task DoWork() {
            
      // Should be false
      Console.WriteLine($"{nameof(IsProcessing)} is {IsProcessing}");

      // At the begin of its lifetime, the Flag<bool> returned by the Flag.Toggle will invert the 
      // initial value of IsProcessing
      using(Flag.Toggle(() => IsProcessing)) { 
        await Task.Run(() => {
                    
          // long running operation
          for(int i = 0; i < 10; i++) {

            //Should be true
            Console.WriteLine($"{nameof(IsProcessing)} is {IsProcessing}");
            Thread.Sleep(1000);
          }
        });

      // At the end of its lifetime, the Flag<bool> will invert the value of IsProcessing again
      }

      // Should be false again
      Console.WriteLine($"{nameof(IsProcessing)} is {IsProcessing}");
    }

    static async Task Main(string[] args) {
        await DoWork();
    }
  }
}
```

### 2. Setting and Resetting values with *[Flag.SetAndReset\<T\>](#flagsetandresettmember-setvalue)*

Use [Flag.SetAndReset\<T\>](#flagsetandresettmember-setvalue) when you need to set an arbitrary value to a field or property momentarily, and reset it back to its original value when done.

```csharp
using Flagger;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Flagger.Samples.SetAndReset {

  class Program {

    //String property to be set
    static string Status { get; set; } = "Idle";

    //Long working method which sets the property during operation to signal the UI
    static async Task DoWork() {

      // Should be "Idle"
      Console.WriteLine($"{nameof(Status)} is {Status}");

      // At the begin of its lifetime, the Flag<string> returned by the Flag.SetAndReset will 
      // store the initial value of Status property ("Idle"), and set it to "Working"
      using(Flag.SetAndReset(() => Status, "Working")) { 
        await Task.Run(() => {

          // long running operation
          for(int i = 0; i < 10; i++) {

            //Should be "Working"
            Console.WriteLine($"{nameof(Status)} is {Status}");
            Thread.Sleep(1000);
          }
        });

      // At the end of its lifetime, the Flag<string> will reset the value of Status back to 
      // "Idle"
      }

      // Should be "Idle" again
      Console.WriteLine($"{nameof(Status)} is {Status}");
    }

    static async Task Main(string[] args) {
      await DoWork();
    }
  }
}
```

### 3. Setting and Resetting values with *[Flag.SetAndUnset\<T\>](#flagsetandunsettmember-setvalue-unsetvalue)*

Use [Flag.SetAndUnset\<T\>](#flagsetandunsettmember-setvalue-unsetvalue) when you need to set an specific initial value to a field or property momentarily, and another specific final value when done.

If you need to set a ``bool`` variable, field or property to ``true`` and unset it to ``false`` (a very common scenario), you can use the simpler [boolean version of the Flag.SetAndUnset](#flagsetandunsetmember) method.

```csharp
using Flagger;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Flagger.Samples.SetAndUnset {
    
  class Program {
    
    //String property to be set
    static string Status { get; set; } = "Idle";

    //Long working method which sets the property during operation to signal the UI
    static async Task DoWork() {

      // Should be "Idle"
      Console.WriteLine($"{nameof(Status)} is {Status}");

      // At the begin of its lifetime, the Flag<string> returned by the Flag.SetAndUnset will 
      // set the Status property to "Working"
      using(Flag.SetAndUnset(() => Status, "Working", "Done")) { 
        await Task.Run(() => {

          // long running operation
          for(int i = 0; i < 10; i++) {

            //Should be "Working"
            Console.WriteLine($"{nameof(Status)} is {Status}");
            Thread.Sleep(1000);
          }
        });

      // At the end of its lifetime, the Flag<string> will setvalue of Status to "Done"
      }

      // Should be "Done"
      Console.WriteLine($"{nameof(Status)} is {Status}");
    }
        
    static async Task Main(string[] args) {
      await DoWork();
    }
  }
}
```

### 4. Setting and Unsetting values only if the initial value is equal to the unset value with *[Flag.SetAndUnsetIfUnset\<T\>](#flagsetandunsetifunsettmember-setvalue-unsetvalue)*

Use [Flag.SetAndUnsetIfUnset\<T\>](#flagsetandunsetifunsettmember-setvalue-unsetvalue) when you need to set an specific initial value to a field or property momentarily, and a specific final value when done, *but only if the initial value is equal to the final value (i.e. it was not set yet)*, in this case, not changing the flag at all.

This is specially useful in recursion scenarios, or when more than one method sets and unsets the same flag.

If you need to set a ``bool`` variable, field or property to ``true`` and unset it to ``false`` only when the initial value of the variable, field or property is ``false`` (another common scenario), you can use the simpler [boolean version of the Flag.SetAndUnsetIfUnset](#flagsetandunsetifunsetmember) method.

```csharp
using Flagger;
using System;

namespace Flagger.Samples.SetAndUnsetIfUnset {

  class Program {

    //String property to be set
    static string Status { get; set; } = "Idle";

    //Recursive method which sets the property during operation to signal the UI
    static void DoWork(int recursionLevel = 0) {
      string padSpaces = new String(' ', recursionLevel);

      // Should be "Idle" in first recursion level, and "Busy" otherwise
      Console.WriteLine($"{padSpaces}At {nameof(recursionLevel)} {recursionLevel}, " + 
                        $"before {nameof(Flag.SetAndUnsetIfUnset)}, {nameof(Status)} is {Status}");

      // At the begin of its lifetime, the Flag<string> returned by the Flag.SetAndUnsetIfUnset will 
      // set the Status property to "Busy" if it is not already "Busy"
      using(Flag.SetAndUnsetIfUnset(() => Status, "Busy", "Done")) { 
        // Should be "Busy"
        Console.WriteLine($"{padSpaces} => At beginning of {nameof(Flag.SetAndUnsetIfUnset)}, " + 
                          $"{nameof(Status)} is {Status}");

        // Call the method recursively
        if(recursionLevel < 3) {
          DoWork(recursionLevel + 1);
        }

        // Should (still) be "Busy"
        Console.WriteLine($"{padSpaces} => At end of {nameof(Flag.SetAndUnsetIfUnset)}, " +
                          $"{nameof(Status)} is {Status}");

      // At the end of its lifetime, the Flag<string> only will set the value of Status to "Done"
      // if it was not previously "Busy"
      }

      // Should be "Done" in first recursion level, and "Busy" otherwise
      Console.WriteLine($"{padSpaces}At {nameof(recursionLevel)} {recursionLevel}, " +
                        $"after {nameof(Flag.SetAndUnsetIfUnset)}, {nameof(Status)} is {Status}");
    }

    static void Main(string[] args) {
      DoWork();
    }
  }
}

```

## Class Reference

| Class | Description |
| ----- | ----- |
| [Flag](#flag) | Contains several methods which allows to configure contextual flags |
| [Flag\<T\>](#flag-t) | Allows for setting a value to the supplied member while the context lasts, and resetting the value on the context disposal |

### Flag

Contains several methods which allows to configure contextual flags

| Method | Description |
| ------ | ----------- |
| [Flag.Toggle (member)](#flagtogglemember) | Toggles the value of a member |
| [Flag.SetAndReset\<T\> (member, setValue)](#flagsetandresettmember-setvalue) | Set and reset the value of a member |
| [Flag.SetAndUnset\<T\> (member, setValue, unsetValue)](#flagsetandunsettmember-setvalue-unsetvalue) | Set and unset the value of a member |
| [Flag.SetAndUnset (member)](#flagsetandunsetmember) | Set and unset the value of a boolean member |
| [Flag.SetAndUnsetIfUnset\<T\> (member, setValue, unsetValue)](#flagsetandunsetifunsettmember-setvalue-unsetvalue) | Set and unset the value of a member if it is currently unset |
| [Flag.SetAndUnsetIfUnset (member)](#flagsetandunsetifunsetmember) | Set and unset the value of a boolean member if it is currently false |

#### Flag.Toggle(member)

Toggles (inverts) the value of the supplied boolean member

| Parameter Name | Description |
| -------------- | ----------- |
| member | *[Expression](https://docs.microsoft.com/dotnet/api/expression-1)\<[Func](https://docs.microsoft.com/dotnet/api/func-1)\<[Boolean](https://docs.microsoft.com/dotnet/api/boolean)\>\>*<br>Lambda expression which indicates the boolean member to be toggled. |

##### Returns

*[Flag\<Boolean\>](#flag-t)*<br>
Flag context which toggles the boolean member

#### Flag.SetAndReset\<T\>(member, setValue)

Sets the value of the supplied boolean member to the provided value, and reset it to its original value on the context disposal

| Parameter Name | Description |
| -------------- | ----------- |
| member | *[Expression](https://docs.microsoft.com/dotnet/api/expression-1)\<[Func](https://docs.microsoft.com/dotnet/api/func-1)\<T\>\>*<br>Lambda expression which indicates the member to be set. |
| setValue | *T*<br>The value to be set at the context creation |

##### Returns

*[Flag\<T\>](#flag-t)*<br>
Flag context which sets and resets the member value

#### Flag.SetAndUnset\<T\>(member, setValue, unsetValue)

Sets the value of the supplied member to the provided set value, and set it to the provided unset value on the context disposal

| Parameter Name | Description |
| -------------- | ----------- |
| member | *[Expression](https://docs.microsoft.com/dotnet/api/expression-1)\<[Func](https://docs.microsoft.com/dotnet/api/func-1)\<T\>\>*<br>Lambda expression which indicates the member to be set. |
| setValue | *\<T\>*<br>The value to be set at the context creation |
| unsetValue | *\<T\>*<br>The value to be set at the contex disposal |

##### Returns

*[Flag\<T\>](#flag-t)*<br>
Flag context which sets and unsets the member value

#### Flag.SetAndUnset(member)

Sets the value of the supplied boolean member to true on context creation, and set it to false on the context disposal

| Parameter Name | Description |
| -------------- | ----------- |
| member | *[Expression](https://docs.microsoft.com/dotnet/api/expression-1)\<[Func](https://docs.microsoft.com/dotnet/api/func-1)\<[Boolean](https://docs.microsoft.com/dotnet/api/boolean)\>\>*<br>Lambda expression which indicates the boolean member to be toggled. |

##### Returns

*[Flag\<Boolean\>](#flag-t)*<br>
Flag context which sets and unsets the member value

#### Flag.SetAndUnsetIfUnset\<T\>(member, setValue, unsetValue)

If the current value of the supplied member is not equal to the setValue parameter value, set it to the setValue on context creation, and set it to the unsetValue parameter value on the context disposal. Otherwise, keeps it unchanged;

| Parameter Name | Description |
| -------------- | ----------- |
| member | *[Expression](https://docs.microsoft.com/dotnet/api/expression-1)\<[Func](https://docs.microsoft.com/dotnet/api/func-1)\<[Boolean](https://docs.microsoft.com/dotnet/api/boolean)\>\>*<br>Lambda expression which indicates the boolean member to be toggled. |
| setValue | *\<T\>*<br>The value to be set at the context creation |
| unsetValue | *\<T\>*<br>The value to be set at the contex disposal |

##### Returns

*[Flag\<T\>](#flag-t)*<br>
Flag context which sets and unsets the member value


#### Flag.SetAndUnsetIfUnset(member)

If the current value of the supplied boolean member is false, set it to true on context creation, and set it to false on the context disposal. Otherwise, keeps it unchanged;

| Parameter Name | Description |
| -------------- | ----------- |
| member | *[Expression](https://docs.microsoft.com/dotnet/api/expression-1)\<[Func](https://docs.microsoft.com/dotnet/api/func-1)\<[Boolean](https://docs.microsoft.com/dotnet/api/boolean)\>\>*<br>Lambda expression which indicates the boolean member to be toggled. |

##### Returns

*[Flag\<Boolean\>](#flag-t)*<br>
Flag context which sets and unsets the member value

### Flag \<T\>

Allows for setting a value to the supplied member while the context lasts, and resetting the value on the context disposal.
You cannot instantiate this class directly, only through one of the [Flag](#flag) static class methods. Also, the only possible interaction with this class, besides creating it, which sets the initial value of a flag, is disposing it, which unsets the value of the flag.
The specific conditional behaviors of *toggling*, *settting-resetting*, *setting-unsetting* and *non-recursively-set-unset* are provided by the [Flag](#flag) static class methods. 

##### Type Parameters

- T - The type of the member to be set and reset.

#### Dispose

Disposes of the flag context, resetting the supplied member value.
