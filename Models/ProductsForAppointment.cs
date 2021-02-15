using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CoreStoreMVC.Models
{
    // Урок 10
    public class ProductsForAppointment
    {
        public int Id { get; set; }
        public int AppointmentId { get; set; }
        public int ProductId { get; set; }

        // Создаём внешний ключ для доставки
        [ForeignKey("AppointmentId")]
        public virtual Appointment Appointments { get; set; }

        // Создаём внешний ключ для заказов
        [ForeignKey("ProductId")]
        public virtual Product Products { get; set; }
    }
}
