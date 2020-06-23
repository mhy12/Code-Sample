using System;
using System.Collections.Generic;
using System.Text;

namespace MicroService.Common.Models
{
    public class MongoDbConnectionSettings
    {
        public string Database { get; set; }

        public string ConnectionString { get; set; }
    }
}
