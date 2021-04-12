
namespace DNA.Domain.Services {
    public interface ICryptoService {
        string CalculateMD5Hash(byte[] buffer);
        bool ValidateMD5Hash(byte[] buffer, string validationEntry);
        string CalculateSHA1Hash(byte[] buffer);
        bool ValidateSHA1Hash(byte[] buffer, string validationEntry);
        string CalculateSHA256Hash(byte[] buffer);
        bool ValidateSHA256Hash(byte[] buffer, string validationEntry);
    }
}
