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
        /// Main loop for the dungeon instance.
        /// The instance repeatedly tries to form a party.
        /// If a party is formed, it simulates a dungeon run (random sleep between t1 and t2 seconds),
        /// updates its counters, and then continues.
        /// When no party can be formed, it sets its status to Done and exits.
        /// </summary>
        public void RunInstance()
        {
            while (true)
            {
                // Signal that this instance is waiting for a party.
                Status = InstanceStatus.Waiting;
                
                // Attempt to form a party.
                if (_lfgManager.TryFormParty(out Party party))
                {
                    // Party formed, start the dungeon run.
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
