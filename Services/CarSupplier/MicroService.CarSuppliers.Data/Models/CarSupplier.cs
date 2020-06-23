using System;
using System.Collections.Generic;
using System.Text;

namespace CarSuppliers.Data.Models
{
    public class CarSupplier
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Website { get; set; }

        public string ContactName { get; set; }

        public string ContactEmail { get; set; }

        public string ContactPhoneNumber { get; set; }

        public IEnumerable<string> CarIds { get; set; }
    }
}
