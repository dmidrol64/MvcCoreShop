using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreStoreMVC.Models
{
    // Урок 10
    public class Appointment
    {
        public int Id { get; set; }
        public DateTime AppointmentDay { get; set; }
        [NotMapped]
        public DateTime AppointmentTime { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhoneNumber { get; set; }
        public string CustomerEmail { get; set; }
        public bool IsConfirmed { get; set; }
    }
}
