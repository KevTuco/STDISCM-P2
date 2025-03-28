using System;
using System.Collections.Generic;
using System.Threading;

namespace P2
{
    public class Scheduler
    {
        private readonly List<DungeonInstance> _instances;
        private bool _shouldStop = false;

        public Scheduler(List<DungeonInstance> instances)
        {
            _instances = instances;
        }

        public void Stop() => _shouldStop = true;

        public void Run()
        {
            int index = 0;
            while (!_shouldStop)
            {
                // Check if all instances are done.
                bool allDone = true;
                foreach (var instance in _instances)
                {
                    if (instance.Status != InstanceStatus.Done)
                    {
                        allDone = false;
                        break;
                    }
                }
                if (allDone)
                    break;

                // Round-robin: signal instance if it is waiting.
                if (_instances[index].Status == InstanceStatus.Empty)
                {
                    _instances[index].Signal();
                }
                index = (index + 1) % _instances.Count;
                Thread.Sleep(100); // Sleep briefly to reduce CPU usage.
            }
        }
    }
}
