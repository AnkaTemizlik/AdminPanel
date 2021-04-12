using DNA.Domain.Extentions;
using DNA.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace DNA.API.Services {

    [Service(typeof(ICryptoService), Lifetime.Scoped)]
    public class CryptoService : ICryptoService {
        public string CalculateMD5Hash(byte[] buffer) {
            using (HashAlgorithm hashAlgorithm = (HashAlgorithm)MD5.Create())
                return BitConverter.ToString(hashAlgorithm.ComputeHash(buffer)).Replace("-", "");
        }

        public string CalculateSHA1Hash(byte[] buffer) {
            using (HashAlgorithm hashAlgorithm = (HashAlgorithm)SHA1.Create())
                return BitConverter.ToString(hashAlgorithm.ComputeHash(buffer)).Replace("-", "");
        }

        public string CalculateSHA256Hash(byte[] buffer) {
            using (HashAlgorithm hashAlgorithm = (HashAlgorithm)SHA256.Create())
                return BitConverter.ToString(hashAlgorithm.ComputeHash(buffer)).Replace("-", "");
        }

        public bool ValidateMD5Hash(byte[] buffer, string validationEntry) {
            using (HashAlgorithm hashAlgorithm = (HashAlgorithm)MD5.Create())
                return string.Equals(BitConverter.ToString(hashAlgorithm.ComputeHash(buffer)).Replace("-", ""), validationEntry);
        }

        public bool ValidateSHA1Hash(byte[] buffer, string validationEntry) {
            using (HashAlgorithm hashAlgorithm = (HashAlgorithm)SHA1.Create())
                return string.Equals(BitConverter.ToString(hashAlgorithm.ComputeHash(buffer)).Replace("-", ""), validationEntry);
        }

        public bool ValidateSHA256Hash(byte[] buffer, string validationEntry) {
            using (HashAlgorithm hashAlgorithm = (HashAlgorithm)SHA256.Create())
                return string.Equals(BitConverter.ToString(hashAlgorithm.ComputeHash(buffer)).Replace("-", ""), validationEntry);
        }
    }
}
