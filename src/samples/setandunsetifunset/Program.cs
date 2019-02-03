using Flagger;
using System;

namespace Flagger.Samples.SetAndUnsetIfUnset {

  class Program {

    //String property to be set
    static string Status { get; set; } = null;

    //Recursive method which sets the property during operation to signal the UI
    static void DoWork(int recursionLevel = 0) {
      string padSpaces = new String(' ', recursionLevel);

      // Should be "Idle" in first recursion level, and "Busy" otherwise
      Console.WriteLine($"{padSpaces}At {nameof(recursionLevel)} {recursionLevel}, " + 
                        $"before {nameof(Flag.SetAndUnsetIfUnset)}, {nameof(Status)} is {Status}");

      // At the begin of its lifetime, the Flag<string> returned by the Flag.SetAndUnsetIfUnset will 
      // set the Status property to "Busy" if it is not already "Busy"
      using(Flag.SetAndUnsetIfUnset(() => Status, "Busy")) { 
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
