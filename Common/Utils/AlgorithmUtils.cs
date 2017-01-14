using System.Text;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
namespace Common.Utils
{
    public class AlgorithmUtils
    {
        public static string ComputeHash(string source, string algorithm)
        {
            HashAlgorithmProvider sha1 = HashAlgorithmProvider.OpenAlgorithm(algorithm);
            byte[] bytes = Encoding.UTF8.GetBytes(source);
            Windows.Storage.Streams.IBuffer bytesBuffer = CryptographicBuffer.CreateFromByteArray(bytes);
            IBuffer hashBuffer = sha1.HashData(bytesBuffer);
            return CryptographicBuffer.EncodeToHexString(hashBuffer);
        }
    }
}
