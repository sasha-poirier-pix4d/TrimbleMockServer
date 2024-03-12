using System.Globalization;

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
        public static SatelliteType[] Values = (SatelliteType[]) Enum.GetValues(typeof(SatelliteType));
        private static readonly Random RNG = new ();
        public static Satellite RandomSatellite() {
            return new Satellite(Id: RNG.Next(),Elv: RNG.Next(0, 180), Azm: RNG.Next(0, 360), Snr: RNG.Next(0, 120), true, Values[RNG.Next(Values.Length)]);
        }
        public static Satellite[] RandomSatellites() {
            var satellites = new Satellite[RNG.Next(0, 10)];
            for (int i = 0; i < satellites.Length; i++)
            {
                satellites[i] = Satellite.RandomSatellite();
            }
            return satellites;
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
        PseudoExtras.Satellite[] satelliteView)
    {
        static PseudoExtrasMessage MakeZero() {
            return new PseudoExtrasMessage(latitude: 0, longitude: 0, altitude: 0, speed: 0, 
                bearing: 0, accuracy: 0, verticalAccuracyMeters: 0, hdop: 0, vdop: 0, pdop: 0,
                diffAge: 0, diffStatus: DiffStatus.Autonomous, diffID: 0,
                vrms: 0, hrms: 0, receiverModel: "", mockProvider: "", geoidModel: "", mslHeight: 0, undulation: 0,
                utcTime: 0, gpsTimeStamp: "", utcTimeStamp: "",
                subscriptionType: SubscriptionType.Free,
                satellites: 0, totalSatInView: 0, 
                satelliteView: []);
        }
        
        public static PseudoExtrasMessage MakeNearMalley(DiffStatus status = DiffStatus.Autonomous) {
            Satellite[] satellites = Satellite.RandomSatellites();
            Random random = new Random();
            double lat = 46.52955048375884 - (random.NextDouble() * (0.001 - -0.001) + -0.001);
            double lon = 6.600880505618732 - (random.NextDouble() * (0.001 - -0.001) + -0.001);
            double alt = 490 - (random.NextDouble() * (0.1 - -0.1) + -0.1);
            double msl = 440 - (random.NextDouble() * (0.1 - -0.1) + -0.1);
            double und = 50 - (random.NextDouble() * (0.1 - -0.1) + -0.1);
            long millis = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            string utcTimeStamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffff", CultureInfo.InvariantCulture);
            string gpsTimeStamp = DateTime.UtcNow.AddSeconds(-20).ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffff", CultureInfo.InvariantCulture);

            float accuracy = 0, accuracyVertical = 0, hdop = 0, vdop = 0, pdop = 0, vrms = 0, hrms = 0;
            switch (status) {
                case DiffStatus.Autonomous :
                    accuracy = Convert.ToSingle(3F + random.NextDouble());
                    accuracyVertical = Convert.ToSingle(4F + random.NextDouble());
                    hdop = pdop = vdop = 2F;
                    hrms = Convert.ToSingle(4F + random.NextDouble());
                    vrms = Convert.ToSingle(3F + random.NextDouble());
                    break;
                case DiffStatus.DGPS:
                    accuracy = Convert.ToSingle(1F + random.NextDouble());
                    accuracyVertical = Convert.ToSingle(2F + random.NextDouble());
                    hrms = Convert.ToSingle(3F + random.NextDouble());
                    vrms = Convert.ToSingle(2F + random.NextDouble());
                    hdop = pdop = vdop = 1F;
                    break;
                case DiffStatus.Float:
                    accuracy = accuracyVertical = Convert.ToSingle(random.NextDouble());
                    vrms = Convert.ToSingle(random.NextDouble());
                    hrms = Convert.ToSingle(random.NextDouble());
                    hdop = pdop = vdop = .2F;
                    break;
                case DiffStatus.Fixed:
                    accuracy = accuracyVertical = .1F;
                    hdop = pdop = vdop = vrms = hrms = .2F;
                    break;
            }

            return new PseudoExtrasMessage(latitude: lat, longitude: lon, altitude: alt, speed: 0, 
                bearing: 0, accuracy: accuracy, verticalAccuracyMeters: accuracyVertical, hdop: hdop, vdop: vdop, pdop: pdop,
                diffAge: 0, diffStatus: status, diffID: 0,
                vrms: vrms, hrms: hrms, receiverModel: "Catalyst", mockProvider: "Trimble Mobile Manager", geoidModel: "EGM96 (Global)", mslHeight: msl, undulation: und,
                utcTime: millis, gpsTimeStamp: gpsTimeStamp, utcTimeStamp: utcTimeStamp,
                subscriptionType: SubscriptionType.Free,
                satellites: satellites.Count(p => p.Use), totalSatInView: satellites.Length,
                satelliteView: satellites);
        }
    }
}
