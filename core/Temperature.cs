using System;
using Dapper.Contrib.Extensions;

namespace core
{
    [Table("TemperatureReadings")]
    public class TemperatureReading
    {
        [Key]
        public int ID { get; set; }

        public Guid LocationID { get; set; }

        public DateTime Timestamp { get; set; }

        public decimal Temperature { get; set; }
    }
}
