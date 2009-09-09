using System;
using System.Collections.Generic;
using FreeSWITCH.Native;

namespace FreeSWITCH.Managed.ParkingLot
{
    public class ParkingLot : IAppPlugin
    {
        public static Dictionary<string, string> ParkedCalls = new Dictionary<string, string>();
        public void Run(AppContext context)
        {
            string entranceNumber = context.Session.GetVariable("parking_lot_entrance_number");
            string firstSpace = context.Session.GetVariable("parking_lot_first_space");
            string spacesCount = context.Session.GetVariable("parking_lot_spaces_count");
            string destinationNumber = context.Session.GetVariable("destination_number");
            if (entranceNumber == destinationNumber)
            {
                foreach (string key in ParkingLot.ParkedCalls.Keys)
                {
                    Log.WriteLine(LogLevel.Info, string.Format("{0}: {1}", key, ParkingLot.ParkedCalls[key]));
                }
                Log.WriteLine(LogLevel.Info, "Park.");
                ParkCall(context.Session, FirstAvailableSpace(firstSpace, spacesCount));
            }
            else
            {
                if (ParkingLot.ParkedCalls.ContainsKey(destinationNumber))
                {
                    Log.WriteLine(LogLevel.Info, "Unpark.");
                    UnParkCall(context.Session);
                }
                return;
            }
        }

        public string FirstAvailableSpace(string firstSpace, string spacesCount)
        {
            if (!ParkingLot.ParkedCalls.ContainsKey(firstSpace)) { return firstSpace; };
            int firstSpaceNumeric = Convert.ToInt32(firstSpace);
            int spacesCountNumeric = Convert.ToInt32(spacesCount);
            for (var i = 1; i < spacesCountNumeric; i++)
            {
                var spaceToTry = firstSpaceNumeric + i;
                if (!ParkingLot.ParkedCalls.ContainsKey(spaceToTry.ToString())) { return spaceToTry.ToString(); };
            }
            return null;
        }

        private void UnParkCall(ManagedSession session)
        {
            string parkingSpace = session.GetVariable("destination_number");
            string parkedCallUuid =ParkingLot.ParkedCalls[parkingSpace];
            session.Answer();
            session.sleep(500, 0);
            session.SetTtsParameters("cepstral", "allison");
            session.Speak("Connecting you.");
            session.Execute("intercept", string.Format("{0}", parkedCallUuid));
            ParkingLot.ParkedCalls.Remove(parkingSpace);
        }

        private void ParkCall(ManagedSession session, string parkingSpace)
        {
            ParkingLot.ParkedCalls[parkingSpace] = session.uuid;
            session.HangupFunction = () => HandleHangup(parkingSpace);
            Log.WriteLine(LogLevel.Info, string.Format("Park: {0}.", parkingSpace));
            session.Answer();
            session.sleep(500, 0);
            session.SetTtsParameters("cepstral", "allison");
            session.Speak("Holding on extension " + parkingSpace+"<break time='1s' />");
            session.sleep(500, 0);
            session.StreamFile("local_stream://moh", -1);
        }

        private void HandleHangup(string parkingSpace)
        {
            Log.WriteLine(LogLevel.Info, string.Format("Removing Park: {0}.", parkingSpace));
            ParkingLot.ParkedCalls.Remove(parkingSpace);
        }
    }
}
