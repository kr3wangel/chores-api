using System;

namespace TodoApi.Models
{
    public class Transaction
    {
        public long TransactionId { get; set; }
        public DateTimeOffset InsertedDate { get; set; }
        public long UserId { get; set; }
        public long ChildId { get; set; }
        public double Amount { get; set; }
        public bool Success { get; set; }
    }
}