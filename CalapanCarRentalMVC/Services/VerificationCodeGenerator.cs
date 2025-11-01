using System.Security.Cryptography;

namespace CalapanCarRentalMVC.Services
{
    public static class VerificationCodeGenerator
    {
        /// <summary>
        /// Generates a random 6-digit verification code
        /// </summary>
        public static string GenerateCode()
    {
  return RandomNumberGenerator.GetInt32(100000, 999999).ToString();
        }

  /// <summary>
        /// Generates a random alphanumeric verification code of specified length
    /// </summary>
        public static string GenerateAlphanumericCode(int length = 6)
   {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
  var code = new char[length];
 
  for (int i = 0; i < length; i++)
       {
 code[i] = chars[RandomNumberGenerator.GetInt32(0, chars.Length)];
            }
   
            return new string(code);
    }
    }
}
