using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeSWITCH.Managed.ParkingLot
{
    public class ParkingLotList : IApiPlugin
    {
        public void Execute(ApiContext context)
        {
            context.Stream.Write("List of Parked Extensions:" + Environment.NewLine);
            if (ParkingLot.ParkedCalls.Count == 0)
            {
                context.Stream.Write("No calls parked." + Environment.NewLine);
            }
            else
            {
                foreach (string key in ParkingLot.ParkedCalls.Keys)
                {
                    context.Stream.Write(string.Format("{0}: {1}{2}", key, ParkingLot.ParkedCalls[key], Environment.NewLine));
                }
            }
        }

        public void ExecuteBackground(ApiBackgroundContext context)
        {
            throw new NotImplementedException();
        }
    }
}
