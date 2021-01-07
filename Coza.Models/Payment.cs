using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Coza.Models
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }
        public int TransactionId { get; set; }
        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; }
        public string TxRef { get; set; }
        public string FlwRef { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public double Amount { get; set; }
        public string Currency { get; set; }
        public string PhoneNumber { get; set; }

        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
    }
}
