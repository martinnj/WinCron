﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinCron
{
    /// <summary>
    /// Class representing a job.
    /// </summary>
    public class CronJob
    {
        private string[] timeParts;
        private string commandString;

        /// <summary>
        /// Constructor for the CronJob class, parses a string representing a line the the crontab file.
        /// </summary>
        /// <param name="cronline"></param>
        public CronJob(string cronline)
        {
            string[] parts = cronline.Trim().Split(' ');
            if (parts.Length < 6) { throw new ArgumentException("Invalid line from cron file."); }
            timeParts = parts.Take(5).ToArray<string>();
            commandString = string.Join(" ", parts.Skip(5));

        }

        public object ExecuteJob()
        {
            return null;
        }


        #region "Should we run this job related things"
        /// <summary>
        /// Ask the job if it is supposed to trigger. This says nothing about the jobs state, it only indicates
        /// that it is time to attempt to run the job.
        /// </summary>
        /// <param name="now">The DateTime to check against.</param>
        /// <returns>true if the job should be run, false otherwise.</returns>
        public bool ShouldRun(DateTime now)
        {
            // TODO: This method gets called every minute for each task, so any optmizations would be awesome.

            int[] nowParts = new int[5] { now.Minute, now.Hour, now.Day, now.Month, (int)now.DayOfWeek };
            bool[] passes = MakeRunClearanceList(timeParts, nowParts);
            // If a single on is false, we can't run.
            foreach (bool b in passes)
            {
                if (!b) { return false; }
            }
            // No false returns, we're good to go.
            return true;
        }

        /// <summary>
        /// Creates a list of booleans indicating wether or not each nowPart is acceptable by the given timeParts.
        /// </summary>
        /// <param name="timeParts"></param>
        /// <param name="nowParts"></param>
        /// <returns></returns>
        private bool[] MakeRunClearanceList(string[] timeParts, int[] nowParts)
        {
            bool[] output = new bool[5] {false, false, false, false, false};

            for (int i = 0; i < 5; i++)
            {
                output[i] = CanRun(timeParts[i], nowParts[i]);
            }

            return output;
        }

        /// <summary>
        /// Determines if a nowValue is acceptable within a given timestringValue.
        /// </summary>
        /// <param name="timestringValue"></param>
        /// <param name="nowValue"></param>
        /// <returns></returns>
        private bool CanRun(string timestringValue, int nowValue)
        {
            ValueType inType = GetValueTypeFromString(timestringValue);
            switch (inType)
            {
                case ValueType.Wildcard:
                    // Always true :)
                    return true;
                case ValueType.Number:
                    // Runnable if they're the same number.
                    return (int.Parse(timestringValue) == nowValue);
                case ValueType.Range:
                    // Runnable if "now" is within the range.
                    return WithinRange(timestringValue, nowValue);
                case ValueType.List:
                    // Runnable if "now" is in the list.
                    return InList(timestringValue, nowValue);
            }
            // Should never get here.
            throw new ArgumentException("Got a strange timestring value: " + timestringValue);
        }

        /// <summary>
        /// Determines if the nowValue is within the given time list.
        /// </summary>
        /// <param name="timestringRange"></param>
        /// <param name="nowValue"></param>
        /// <returns>true if so, false if not.</returns>
        private bool InList(string timestringRange, int nowValue)
        {
            string[] parts = timestringRange.Split(',');
            foreach (string part in parts)
            {
                if (nowValue == int.Parse(part)) { return true; }
            }
            return false;
        }

        /// <summary>
        /// Takes a string on the form "number-number" and a number and assesses
        /// wether or not the single number is within the given range.
        /// </summary>
        /// <param name="timestringRange"></param>
        /// <param name="nowValue"></param>
        /// <returns>true if it is, false if not.</returns>
        private bool WithinRange(string timestringRange, int nowValue)
        {
            string[] parts = timestringRange.Split('-');
            return ((nowValue >= int.Parse(parts[0])) && (nowValue <= int.Parse(parts[1])));
        }

        /// <summary>
        /// Tries to guess if a time entry is a list, a range, a wildcard or a number.
        /// Naiveimplementation that checks for key characters for each type.
        /// </summary>
        /// <param name="input">A string to guess from.</param>
        /// <returns>A ValueType enum depending on the guess.</returns>
        private ValueType GetValueTypeFromString(string input)
        {
            if (input.Contains(",")) { return ValueType.List; }
            if (input.Contains("-")) { return ValueType.Range; }
            if (input.Contains("*")) { return ValueType.Wildcard; }
            return ValueType.Number;
        }

        private enum ValueType
        {
            /// <summary>
            /// Pure simple number, ex: "3"
            /// </summary>
            Number,
            /// <summary>
            /// Wildcard, ex: "*"
            /// </summary>
            Wildcard,
            /// <summary>
            /// A range on the form "number-number"
            /// </summary>
            Range,
            /// <summary>
            /// A list of numbers on the form "number,number,number
            /// </summary>
            List
        }
        #endregion
    }
}