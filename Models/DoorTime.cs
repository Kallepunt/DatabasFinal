using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatabasFinal.Models
{
    public class DoorTime
    {

        public DateTime DateTime { get; set; }

        public TimeSpan timeSpan { get; set; }

        public double AvgTempIn { get; set; }

        public double AvgTempOut { get; set; }


    }
}
