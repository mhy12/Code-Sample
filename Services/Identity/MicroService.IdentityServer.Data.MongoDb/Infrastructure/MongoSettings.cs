using System;
using System.Collections.Generic;
using System.Text;

namespace MicroService.IdentityServer.Data.MongoDb.Infrastructure
{
    public class MongoSettings
    {
        public string ConnectionString { get; set; }

        public string DatabaseName { get; set; }
    }
}
