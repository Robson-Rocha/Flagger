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
