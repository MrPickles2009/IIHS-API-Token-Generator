using System;
using System.IO;
using System.Linq;
using System.Text;

namespace iihsTokenGenerator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Enter account name:");
            String accountName = Console.ReadLine();
            Console.WriteLine("Enter api key:");
            String apiKey = Console.ReadLine();
            
            var nonce = new byte[8];
            new System.Security.Cryptography.RNGCryptoServiceProvider().GetBytes(nonce);
            
            DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var expiryTime = DateTime.UtcNow.AddMinutes(30);
            var seconds = (long)((expiryTime - Epoch).TotalSeconds);
            var expiryBytes = BitConverter.GetBytes(seconds);
            
            var accountNameBytes = Encoding.UTF8.GetBytes(accountName);
            var apiKeyBytes = Encoding.UTF8.GetBytes(apiKey);
            var bytesToHash = accountNameBytes
                .Concat(apiKeyBytes)
                .Concat(nonce)
                .Concat(expiryBytes)
                .ToArray();
            var hash = new System.Security.Cryptography.SHA1CryptoServiceProvider().ComputeHash(bytesToHash);
            
            var token = expiryBytes
                .Concat(nonce)
                .Concat(hash).ToArray();
            var tokenString = (Convert.ToBase64String(token));
            tokenString = tokenString.Replace('+', '-').Replace('/', '_').Replace("=", "");

            Console.WriteLine("Your token is:");
            Console.Write(tokenString);
            Console.WriteLine("\n\nPress 'enter' to save the token in the textfile apiToken.txt in the current directory");
            Console.ReadLine();

            FileStream apiToken = new FileStream("apiToken.txt", FileMode.Create);
            var streamwriter = new StreamWriter(apiToken);
            streamwriter.AutoFlush = true;
            Console.SetOut(streamwriter);
            Console.Write(tokenString);
            Console.SetError(streamwriter);
        }
    }
}
