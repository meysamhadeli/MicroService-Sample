using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using MicroPack.CQRS.Commands;
using MicroPack.Security;

namespace Pacco.Services.Availability.Infrastructure.Decorators
{
    public class SecurityDecorator<T> : ICommandHandler<T> where T : class, ICommand
    {
        private readonly ICommandHandler<T> _handler;
        private readonly IHasher _hasher;
        private readonly IEncryptor _encryptor;
        private readonly ISigner _signer;

        public SecurityDecorator(ICommandHandler<T> handler ,IHasher hasher, IEncryptor encryptor, ISigner signer)
        {
            _handler = handler;
            _hasher = hasher;
            _encryptor = encryptor;
            _signer = signer;
        }
        public Task HandleAsync(T command)
        {
            var text = "devmentor.io";
            var secretKey = "WN2?!7nxmmTpvfPWxXYkd)&etNEWpxBh";
            var hash = _hasher.Hash(text);
            
           var encriptedText = _encryptor.Encrypt(text, secretKey);
           var decriptedText = _encryptor.Decrypt(encriptedText, secretKey);
           var privateKey = new X509Certificate2("certs/localhost.pfx", "test"); //private key
           var data = new DataDto()
           {
              Id = 1,  
              Name = "devmentors"
           };
           var signature = _signer.Sign(data, privateKey);
           var publicKey = new X509Certificate2("certs/localhost.cer");  //public key

           var isValid = _signer.Verify(data, publicKey, signature);
           
            return  _handler.HandleAsync(command);
        }
        
        [Serializable]
        private class DataDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }


}