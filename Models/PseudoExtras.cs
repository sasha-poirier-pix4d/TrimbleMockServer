/**
Basically copy from Swift project
*/
namespace PseudoExtras {
    public enum DiffStatus
    {
        Autonomous = 1, DGPS = 2, Fixed = 4, Float = 5, Unknown
    }

    public enum SubscriptionType
    {
        Free, Meter, Submeter, Decimeter, Precision, PrecisionOnDemand, GNSS = 100, Unknown
    }

    public enum SatelliteType
    {
        GPS, SBAS, GLONASS, OMNISTAR, GALILEO, BEIDOU, QZSS, IRNSS
    }

    record Satellite(
        int Id,  int Elv, int Azm, int Snr, bool Use, SatelliteType Type
    ) {
        public static Array Values = Enum.GetValues(typeof(SatelliteType));
        private static readonly Random RNG = new ();
        public static Satellite Random() {
            return new Satellite(Id: RNG.Next(),Elv: RNG.Next(0, 180), Azm: RNG.Next(0, 360), Snr: RNG.Next(0, 120), true, (SatelliteType)Values.GetValue(RNG.Next(Values.Length)));
        }
    }

    record PseudoExtrasMessage(
        double latitude, double longitude, double altitude,
        float speed, float bearing,
        float accuracy, float verticalAccuracyMeters, float hdop, float vdop, float pdop,
        float diffAge, DiffStatus diffStatus, int diffID,
        float vrms, float hrms,
        string receiverModel, string mockProvider,
        string geoidModel, double mslHeight, double undulation,
        float utcTime, string gpsTimeStamp, string utcTimeStamp,
        PseudoExtras.SubscriptionType subscriptionType,
        int satellites, int totalSatInView, 
        //Incorrect structure from documentation
        //int satellitesView,
        //int[] satellitesId, int[] satellitesElv, int[] satellitesAzm, int[] satellitesSnr, bool[] satellitesUse, SatelliteType[] satellitesType/*, Satellite[] satellitesMerged*/)
        PseudoExtras.Satellite[] satelliteView)
    {
        static PseudoExtrasMessage MakeZero() {
            return new PseudoExtrasMessage(latitude: 0, longitude: 0, altitude: 0, speed: 0, 
                bearing: 0, accuracy: 0, verticalAccuracyMeters: 0, hdop: 0, vdop: 0, pdop: 0,
                diffAge: 0, diffStatus: DiffStatus.Unknown, diffID: 0,
                vrms: 0, hrms: 0, receiverModel: "", mockProvider: "", geoidModel: "", mslHeight: 0, undulation: 0,
                utcTime: 0, gpsTimeStamp: "", utcTimeStamp: "",
                subscriptionType: SubscriptionType.Free,
                satellites: 0, totalSatInView: 0, 
                satelliteView: []);
                //satellitesView: 0, satellitesId: new int[0], satellitesElv: new int[0], satellitesAzm: new int[0], satellitesSnr: new int[0], satellitesUse: new bool[0], satellitesType: new SatelliteType[0]);
        }
        
        public static PseudoExtrasMessage MakeNearMalley() {
            //Variance
            Random random = new Random();
            var lat = 46.52955048375884 - (random.NextDouble() * (0.001 - -0.001) + -0.001);
            var lon = 6.600880505618732 - (random.NextDouble() * (0.001 - -0.001) + -0.001);
            var millis = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

            //After 10 messages or so, send corrected...
            return new PseudoExtrasMessage(latitude: lat, longitude: lon, altitude: 400, speed: 0, 
                bearing: 0, accuracy: 0, verticalAccuracyMeters: 0, hdop: 0, vdop: 0, pdop: 0,
                diffAge: 0, diffStatus: DiffStatus.Unknown, diffID: 0,
                vrms: 1, hrms: 1, receiverModel: "Catalyst", mockProvider: "Trimble Mobile Manager", geoidModel: "Some Model", mslHeight: 400, undulation: 20,
                utcTime: millis, gpsTimeStamp: "2024-02-13T14:49:49.0000000", utcTimeStamp: "2024-02-13T14:49:49.0000000",
                subscriptionType: SubscriptionType.Free,
                satellites: 0, totalSatInView: 0,
                satelliteView: [new Satellite(1, 20, 20, 20, false, SatelliteType.GPS)]);
                //satellitesView: 0, satellitesId: new int[0], satellitesElv: new int[0], satellitesAzm: new int[0], satellitesSnr: new int[0], satellitesUse: new bool[0], satellitesType: new SatelliteType[0]);
        }
    }
}
