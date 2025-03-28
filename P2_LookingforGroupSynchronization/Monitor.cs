using System;
using System.Collections.Generic;
using System.Threading;

namespace P2
{
    public class Monitor
    {
        private readonly List<DungeonInstance> _instances;
        private readonly LFGManager _lfgManager;
        private bool _shouldStop = false;

        public Monitor(List<DungeonInstance> instances, LFGManager lfgManager)
        {
            _instances = instances;
            _lfgManager = lfgManager;
        }

        public void Stop() => _shouldStop = true;

        public void Run()
        {
            while (!_shouldStop)
            {
                Console.Clear();
                Console.WriteLine("Time: " + DateTime.Now.ToString("HH:mm:ss"));
                Console.WriteLine("---------------------------------------------------");
                Console.WriteLine($"Threads: {_instances.Count}, Total Players: Tanks={_lfgManager.RemainingTanks}, Healers={_lfgManager.RemainingHealers}, DPS={_lfgManager.RemainingDps}");
                Console.WriteLine("---------------------------------------------------");
                foreach (var inst in _instances)
                {
                    Console.WriteLine($"Instance {inst.InstanceId}: Status: {inst.Status}, Parties Served: {inst.PartiesCompleted}, Total Time Served: {inst.TotalTimeServed.TotalSeconds:F1} sec");
                }
                Console.WriteLine("---------------------------------------------------");
                Thread.Sleep(1000); // Update every second.

                // Stop if all instances are done.
                bool allDone = true;
                foreach (var inst in _instances)
                {
                    if (inst.Status != InstanceStatus.Done)
                    {
                        allDone = false;
                        break;
                    }
                }
                if (allDone)
                    _shouldStop = true;
            }
        }
    }
}
