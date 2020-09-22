using System;
using System.Collections.Generic;
using System.Text;

namespace App1_malliksi.Models
{
    class TrackerControlModel
    {
        public int TrackerID { get; set; }
        public string LastChangeTime { get; set; }
        public string RelayControl { get; set; }
        public string GpsActive { get; set; }
        public string Queue { get; set; }
    }
}
