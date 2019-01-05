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
