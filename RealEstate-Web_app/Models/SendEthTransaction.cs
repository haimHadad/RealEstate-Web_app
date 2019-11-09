using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RealEstate_Web_app.Models
{
    public class SendEthTransaction
    {
        [Required]
        public String AccountTo { get; set; }
        [Required]
        public double Ammount { get; set; }

        public SendEthTransaction(String _to, double _value )
        {
            AccountTo = _to;
            Ammount = _value;
        }

    }
}
