using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersistentReaper
{
    public class Scent
    {
        public int scentIntensity = 0;
        public Scent()
        {
            scentIntensity = PersistentReaperPatcher.PRConfig.scentLifetime;
            expireScent();
        }

        public void refreshScent()
        {
            scentIntensity = PersistentReaperPatcher.PRConfig.scentLifetime;
        }

        public async void expireScent()
        {
            while(scentIntensity > 0)
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                scentIntensity--;
                if(scentIntensity < 0)
                {
                    scentIntensity = 0;
                }
            }
        }
    }
}
