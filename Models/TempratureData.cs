using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DatabasFinal.Models
{
    public class TempratureData
    {


        [Key]
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public double Temperature { get; set; }
        public int Humidity { get; set; }
        
        public double MouldRisk { get; set; }
        public string Location { get; set; }

       



    }
}
