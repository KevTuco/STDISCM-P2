using System;
using System.IO;
using System.Collections.Generic;

namespace P2
{
    public static class InputValidator
    {
        /// <summary>
        /// Reads and validates input from "input.txt". The file must have exactly six lines in the format:
        /// n: <value>
        /// t: <value>
        /// h: <value>
        /// d: <value>
        /// t1: <value>
        /// t2: <value>
        /// 
        /// For maximum range, we use ulong for n, t, h, d and uint for t1 and t2.
        /// Negative time values are explicitly rejected.
        /// </summary>
        /// <returns>
        /// A tuple containing (n, t, h, d, t1, t2) if valid; otherwise, returns null.
        /// </returns>
        public static (ulong n, ulong t, ulong h, ulong d, uint t1, uint t2)? ReadAndValidateInput()
        {
            string filePath = "input.txt";
            if (!File.Exists(filePath))
            {
                Console.WriteLine("Error: Input file 'input.txt' not found.");
                return null;
            }

            string[] lines = File.ReadAllLines(filePath);
            if (lines.Length != 6)
            {
                Console.WriteLine("Error: Input file must contain exactly 6 lines (n, t, h, d, t1, t2).");
                return null;
            }

            // Read the file into a dictionary (keys are case-insensitive).
            Dictionary<string, string> variables = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                string[] parts = line.Split(':');
                if (parts.Length != 2)
                {
                    Console.WriteLine($"Error: Line '{line}' is not in the correct 'Key: Value' format.");
                    return null;
                }
                string key = parts[0].Trim();
                string value = parts[1].Trim();
                if (variables.ContainsKey(key))
                {
                    Console.WriteLine($"Error: Duplicate variable '{key}' found.");
                    return null;
                }
                variables[key] = value;
            }

            // Ensure all required keys are present.
            string[] requiredKeys = { "n", "t", "h", "d", "t1", "t2" };
            foreach (string key in requiredKeys)
            {
                if (!variables.ContainsKey(key))
                {
                    Console.WriteLine($"Error: Missing required variable '{key}'.");
                    return null;
                }
            }

            // Use ulong for n, t, h, d.
            if (!ulong.TryParse(variables["n"], out ulong n) || n < 1)
            {
                Console.WriteLine("Error: 'n' (maximum concurrent instances) must be a positive integer, 0 instances is not allowed.");
                return null;
            }
            if (!ulong.TryParse(variables["t"], out ulong tanks))
            {
                Console.WriteLine("Error: 't' (number of tank players) must be a valid positive integer.");
                return null;
            }
            if (!ulong.TryParse(variables["h"], out ulong healers))
            {
                Console.WriteLine("Error: 'h' (number of healer players) must be a valid positive integer.");
                return null;
            }
            if (!ulong.TryParse(variables["d"], out ulong dps))
            {
                Console.WriteLine("Error: 'd' (number of DPS players) must be a valid positive integer.");
                return null;
            }

            // Use uint for time values.
            // Check for a negative sign before attempting to parse.
            if (variables["t1"].Trim().StartsWith("-"))
            {
                Console.WriteLine("Error: 't1' (minimum dungeon clear time) cannot be negative.");
                return null;
            }
            if (variables["t2"].Trim().StartsWith("-"))
            {
                Console.WriteLine("Error: 't2' (maximum dungeon clear time) cannot be negative.");
                return null;
            }
            if (!uint.TryParse(variables["t1"], out uint t1))
            {
                Console.WriteLine("Error: 't1' must be a valid non-negative integer.");
                return null;
            }
            if (!uint.TryParse(variables["t2"], out uint t2))
            {
                Console.WriteLine("Error: 't2' must be a valid non-negative integer.");
                return null;
            }
            if (t2 < t1)
            {
                Console.WriteLine("Error: 't2' must be greater than or equal to 't1'.");
                return null;
            }

            return (n, tanks, healers, dps, t1, t2);
        }
    }
}
