using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.IO;

namespace ThirdPerson
{
    public static class PerVehicleConfig
    {
        private static bool dirty = false;
        private const float defaultZoom = 5.1f;
        private static Dictionary<string, float> Distances = new Dictionary<string, float>();
        public static float GetDistance()
        {
            return Distances.GetOrDefault(FindName(), defaultZoom);
        }
        public static void UpdateDistance(float distance)
        {
            if(Distances.ContainsKey(FindName()))
            {
                float storedDistance = Distances[FindName()];
                float distanceDifference = Mathf.Abs(distance - storedDistance);
                if (distanceDifference > 0.01)
                {
                    Distances[FindName()] = distance;
                    dirty = true;
                }
            }
            else
            {
                Distances[FindName()] = distance;
                dirty = true;
            }
        }
        public static string FindName()
        {
            string name;
            Vehicle myVehicle = Player.main.GetVehicle();
            SubRoot mySub = Player.main.currentSub;
            if (myVehicle != null && Player.main.mode == Player.Mode.LockedPiloting)
            {
                name = myVehicle.ToString();
            }
            else if (mySub != null && Player.main.mode == Player.Mode.Piloting)
            {
                name = mySub.ToString();
            }
            else // player without vehicle
            {
                name = "player";
            }
            return name;
        }
        public static void Load()
        {
            try
            {
                Distances = JsonInterface.Read();
            }
            catch (FileNotFoundException)
            {
                Logger.Log("No configuration file found. Will create one.");
            }
            catch(Exception e)
            {
                Logger.Error(e.Message);
            }
        }
        public static void Save()
        {
            if (dirty)
            {
                JsonInterface.Write(Distances);
            }
            dirty = false;
        }
    }
}
