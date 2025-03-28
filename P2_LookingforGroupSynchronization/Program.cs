using System;
using System.Collections.Generic;
using System.Threading;

namespace P2
{
    class Program
    {
        static void Main(string[] args)
        {
            // 1. Input Validation
            var inputParams = InputValidator.ReadAndValidateInput();
            if (inputParams == null)
                return;

            // Extract validated parameters.
            ulong n = inputParams.Value.n;
            ulong tanks = inputParams.Value.t;
            ulong healers = inputParams.Value.h;
            ulong dps = inputParams.Value.d;
            uint t1 = inputParams.Value.t1;
            uint t2 = inputParams.Value.t2;

            // 2. Initialize LFGManager with player counts.
            LFGManager manager = new LFGManager(tanks, healers, dps);

            // 3. Create dungeon instances (each will run on its own thread).
            List<DungeonInstance> instances = new List<DungeonInstance>();
            for (int i = 0; i < (int)n; i++)
            {
                DungeonInstance instance = new DungeonInstance(i, t1, t2, manager);
                instances.Add(instance);
            }

            // 4. Start dungeon instance threads.
            List<Thread> instanceThreads = new List<Thread>();
            foreach (var instance in instances)
            {
                Thread t = new Thread(instance.RunInstance);
                t.Start();
                instanceThreads.Add(t);
            }

            // 5. Start the Scheduler thread (round-robin scheduler).
            Scheduler scheduler = new Scheduler(instances);
            Thread schedulerThread = new Thread(scheduler.Run);
            schedulerThread.Start();

            // 6. Start the Monitor thread (polls and prints status updates).
            Monitor monitor = new Monitor(instances, manager);
            Thread monitorThread = new Thread(monitor.Run);
            monitorThread.Start();

            // Wait for all dungeon instance threads to finish.
            foreach (var t in instanceThreads)
            {
                t.Join();
            }
            // Wait for scheduler and monitor threads to finish.
            schedulerThread.Join();
            monitorThread.Join();

            // Final Summary
            Console.WriteLine("Final Summary:");
            Console.WriteLine($"Remaining Players: Tanks={manager.RemainingTanks}, Healers={manager.RemainingHealers}, DPS={manager.RemainingDps}");
            foreach (var instance in instances)
            {
                Console.WriteLine($"Instance {instance.InstanceId}: Status: {instance.Status}, Parties Served: {instance.PartiesCompleted}, Total Time Served: {instance.TotalTimeServed.TotalSeconds:F1} sec");
            }
        }
    }
}
