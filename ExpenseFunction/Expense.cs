using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseFunction
{
    public class Expense
    {
        public DateTime Date { get; set; }
        public string Reference { get; set; }
        public string Description { get; set; }
        public string CardMember { get; set; }
        public string CardNumber { get; set; }
        public double Amount { get; set; }
        public string Category { get; set; }
        public string ChargeType { get; set; }


    }
}
