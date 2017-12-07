using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Satochat.Shared.Crypto;

namespace Satochat.Client.Service
{
    public class PublicKeyStore {
        private readonly SatoService _service;

        public PublicKeyStore(SatoService service) {
            _service = service;
        }

        public async Task<SatoPublicKey> GetKeyAsync(string userUuid) {
            // TODO: get from cache
            var key = await _service.GetPublicKey(userUuid);
            return key;
        }
    }
}
