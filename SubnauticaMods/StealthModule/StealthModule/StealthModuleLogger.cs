using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace StealthModule
{
    internal class StealthModuleLogger : MonoBehaviour
    {
        private Dictionary<Creature, LogEntry> LogDict = new Dictionary<Creature, LogEntry>();
        private bool waiting = false;
        internal void Add(Creature creat, string name, float distance)
        {
            var newEntry = new LogEntry(name, distance);
            if(LogDict.ContainsKey(creat))
            {
                if(newEntry.distance < LogDict[creat].distance)
                {
                    LogDict[creat] = newEntry;
                }
            }
            else
            {
                LogDict[creat] = newEntry;
            }
        }
        private void Update()
        {
            if(waiting || LogDict.Count() == 0)
            {
                return;
            }
            UWE.CoroutineHost.StartCoroutine(LogOnce());

        }
        private IEnumerator LogOnce()
        {
            const float timeToWait = 1f;
            waiting = true;
            var entryList = LogDict.Values.ToList();
            entryList.Sort();
            var entry = entryList.First();
            VehicleFramework.Logger.Output(entry.name + Mathf.RoundToInt(entry.distance).ToString() + "m", time: timeToWait);
            LogDict.Clear();
            yield return new WaitForSeconds(timeToWait);
            waiting = false;
        }
    }
    internal struct LogEntry : IComparable<LogEntry>
    {
        internal string name;
        internal float distance;
        internal LogEntry(string nam, float dist)
        {
            name = nam;
            distance = dist;
        }
        public int CompareTo(LogEntry otherEntry)
        {
            return distance.CompareTo(otherEntry.distance);
        }
    }
}
