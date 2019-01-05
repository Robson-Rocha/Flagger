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
