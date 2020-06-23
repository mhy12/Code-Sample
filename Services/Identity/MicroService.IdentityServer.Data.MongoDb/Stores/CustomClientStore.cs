using IdentityServer4.Models;
using IdentityServer4.Stores;
using MicroService.IdentityServer.Data.MongoDb.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MicroService.IdentityServer.Data.MongoDb.Stores
{
    public class CustomClientStore : IClientStore
    {
        protected IMongoRepository _dbRepository;

        public CustomClientStore(IMongoRepository repository)
        {
            _dbRepository = repository;
        }

        public Task<Client> FindClientByIdAsync(string clientId)
        {
            var client = _dbRepository.Single<Client>(c => c.ClientId == clientId);

            return Task.FromResult(client);
        }
    }
}
