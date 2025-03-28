using System;
using System.Threading;

namespace P2
{
    // Enumeration for the instance status.
    public enum InstanceStatus
    {
        Waiting,
        Running,
        Done
    }

    public class DungeonInstance
    {
        private readonly LFGManager _lfgManager;
        private readonly uint _t1; // minimum run time (seconds)
        private readonly uint _t2; // maximum run time (seconds)
        private readonly Random _random;
        private readonly AutoResetEvent _signal = new AutoResetEvent(false);

        public int InstanceId { get; }
        public InstanceStatus Status { get; private set; }
        public int PartiesCompleted { get; private set; }
        public TimeSpan TotalTimeServed { get; private set; }

        public DungeonInstance(int instanceId, uint t1, uint t2, LFGManager lfgManager)
        {
            InstanceId = instanceId;
            _t1 = t1;
            _t2 = t2;
            _lfgManager = lfgManager;
            // Seed Random based on instanceId and current time to ensure varied runs.
            _random = new Random(instanceId + Environment.TickCount);
            Status = InstanceStatus.Waiting;
            PartiesCompleted = 0;
            TotalTimeServed = TimeSpan.Zero;
        }

        /// <summary>
        /// Signals this instance to attempt forming a party.
        /// </summary>
        public void Signal()
        {
            _signal.Set();
        }

        /// <summary>
        /// Main loop for the dungeon instance.
        /// The instance waits for a signal (from the scheduler) before trying to form a party.
        /// If a party is formed, it simulates a dungeon run (random sleep between t1 and t2 seconds),
        /// updates its counters, and then continues.
        /// When no party can be formed, it sets its status to Done and exits.
        /// </summary>
        public void RunInstance()
        {
            while (true)
            {
                // Wait for a signal from the scheduler.
                _signal.WaitOne();
                // Set status to Waiting to indicate readiness to form a party.
                Status = InstanceStatus.Waiting;
                
                // Attempt to form a party.
                if (_lfgManager.TryFormParty(out Party? party))
                {
                    // Party formed; simulate dungeon run.
                    Status = InstanceStatus.Running;
                    
                    // Generate a random run time between t1 and t2 (inclusive).
                    int runTimeSeconds = _random.Next((int)_t1, (int)_t2 + 1);
                    
                    // Simulate dungeon run.
                    Thread.Sleep(runTimeSeconds * 1000);
                    
                    // Update counters.
                    PartiesCompleted++;
                    TotalTimeServed += TimeSpan.FromSeconds(runTimeSeconds);
                }
                else
                {
                    // Not enough players remain to form a party; finish this instance.
                    Status = InstanceStatus.Done;
                    break;
                }
            }
        }
    }
}
