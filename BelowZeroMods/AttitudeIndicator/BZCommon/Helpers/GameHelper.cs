using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace BZCommon.Helpers
{
    public class GameHelper
    {   
        public void SetProgressColor(Color color)
        {
            HandReticle.main.progressText.color = color;
            HandReticle.main.progressImage.color = color;
        }

        public void SetInteractColor(Color color, bool isSetSecondary = true)
        {
            HandReticle.main.compTextHand.color = color;

            if (isSetSecondary)
                HandReticle.main.compTextUse.color = color;
        }
        
        public int GetSlotIndex(Vehicle vehicle, TechType techType)
        {
            InventoryItem inventoryItem = null;

            for (int i = 0; i < vehicle.GetSlotCount(); i++)
            {
                try
                {
                    inventoryItem = vehicle.GetSlotItem(i);
                }
                catch
                {
                    continue;
                }

                if (inventoryItem != null && inventoryItem.item.GetTechType() == techType)
                {
                    return vehicle.GetType() == typeof(Exosuit) ? i - 2 : i;
                }
            }

            return -1;
        }        
    }
}
