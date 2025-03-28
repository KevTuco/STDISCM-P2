using System;

namespace P2
{
    // Simple class representing a formed party.
    public class Party
    {
        // Additional details could be added if needed.
    }

    public class LFGManager
    {
        // Numeric values representing the available players in each role.
        private ulong _tanks;
        private ulong _healers;
        private ulong _dps;

        // Lock to protect the shared player counts.
        private readonly object _lock = new object();

        public LFGManager(ulong tanks, ulong healers, ulong dps)
        {
            _tanks = tanks;
            _healers = healers;
            _dps = dps;
        }

        // Expose the remaining counts as thread-safe properties.
        public ulong RemainingTanks 
        { 
            get { lock (_lock) { return _tanks; } } 
        }
        public ulong RemainingHealers 
        { 
            get { lock (_lock) { return _healers; } } 
        }
        public ulong RemainingDps 
        { 
            get { lock (_lock) { return _dps; } } 
        }

        /// <summary>
        /// Atomically checks if there are enough players to form a party (1 tank, 1 healer, 3 DPS).
        /// If so, it decrements the counts and returns a new Party object.
        /// Returns false if a party cannot be formed.
        /// </summary>
        public bool TryFormParty(out Party? party)
        {
            lock (_lock)
            {
                if (_tanks >= 1 && _healers >= 1 && _dps >= 3)
                {
                    _tanks -= 1;
                    _healers -= 1;
                    _dps -= 3;
                    party = new Party();
                    return true;
                }
                else
                {
                    party = null;
                    return false;
                }
            }
        }
    }
}
