using System;

namespace Restaurante.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public string CustomerName { get; set; }
        public int TableId { get; set; }
    }
}
