using System;

namespace P2
{
    class Program
    {
        static void Main(string[] args)
        {
            var inputParams = InputValidator.ReadAndValidateInput();
            if (inputParams == null)
            {
                // Input validation failed – error messages were already displayed.
                return;
            }
            
            // Input is valid – display the validated parameters.
            Console.WriteLine("Input is valid. Ready for simulation.");
            Console.WriteLine($"n: {inputParams.Value.n}");
            Console.WriteLine($"t: {inputParams.Value.t}");
            Console.WriteLine($"h: {inputParams.Value.h}");
            Console.WriteLine($"d: {inputParams.Value.d}");
            Console.WriteLine($"t1: {inputParams.Value.t1}");
            Console.WriteLine($"t2: {inputParams.Value.t2}");
        }
    }
}
