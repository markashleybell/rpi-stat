using System;

namespace rpi_stat_ui.Models
{
    public class Reading
    {
        public Guid LocationID { get; set; }

        public decimal Temperature { get; set; }
    }
}
