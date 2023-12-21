namespace API.Providers.ResponseModels
{
    namespace TomorrowIO
    {
        // Autogenerated with https://json2csharp.com/
        public class Data
        {
            public List<Timeline> timelines { get; set; }
        }

        public class Interval
        {
            public DateTime startTime { get; set; }
            public Values values { get; set; }
        }

        public class Root
        {
            public Data data { get; set; }
        }

        public class Timeline
        {
            public string timestep { get; set; }
            public DateTime endTime { get; set; }
            public DateTime startTime { get; set; }
            public List<Interval> intervals { get; set; }
        }

        public class Values
        {
            public double cloudCover { get; set; }
            public double humidity { get; set; }
            public double precipitationIntensity { get; set; }
            public double precipitationType { get; set; }
            public double temperature { get; set; }
            public double windDirection { get; set; }
            public double windSpeed { get; set; }
        }
    }
}