using System.Security.Cryptography;
using System.Text;

namespace MepasTask.UI.Helpers
{
    public static class CookieHandler
    {
        // Simetrik şifreleme için kullanılacak gizli anahtar (32 byte uzunluğunda)
        private static readonly byte[] Key = new byte[32]
{
    0x13, 0x24, 0x57, 0x8A, 0xBC, 0xDE, 0xF1, 0x23,
    0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF, 0x01, 0x23,
    0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF, 0x01, 0x23,
    0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF, 0x01, 0x23
};

        public static void TumCerezleriTemizle(HttpContext context)
        {
            var cookies = context.Request.Cookies.Keys;
            foreach (var cookie in cookies)
            {
                context.Response.Cookies.Delete(cookie);
            }
        }

        public static string CerezOku(HttpContext context, string key)
        {
            var encryptedValue = context.Request.Cookies[key];
            if (encryptedValue != null)
            {
                return Decrypt(encryptedValue);
            }
            return null;
        }

        public static void CerezYaz(HttpContext context, string key, string value)
        {
            var encryptedValue = Encrypt(value);
            DateTime expirationDate = DateTime.UtcNow.AddDays(7);

            // Create a new cookie with the specified key, value, and expiration date
            CookieOptions options = new CookieOptions
            {
                
                Expires = expirationDate,
                HttpOnly= true,
                Secure = true,
                // You can also set other options like HttpOnly, Secure, etc., if needed.
                // For example, HttpOnly = true will make the cookie inaccessible from JavaScript.
                // HttpOnly = true,
                // Secure = true, // Only sent over HTTPS if enabled
            };
            context.Response.Cookies.Append(key, encryptedValue);
        }

        public static void CerezSil(HttpContext context, string key)
        {
            // CookieOptions ile çerezin süresini ve diğer özelliklerini ayarlayabilirsiniz.
            // Silmek istediğiniz çerez için aynı anahtar (key) ve Path ile oluşturduğunuz bir çerez oluşturup,
            // son kullanma tarihini geçmiş bir tarih olarak ayarlayarak çerezi silebilirsiniz.
            CookieOptions options = new CookieOptions
            {
                Expires = DateTime.UnixEpoch, // Geçmiş bir tarih
                HttpOnly = true,
                Secure = true,
                // Diğer çerez ayarları burada da uygulanabilir.
            };

            // Response.Cookies.Delete metoduyla çerezi silebilirsiniz.
            context.Response.Cookies.Delete(key, options);
        }

        private static string Encrypt(string value)
        {
            byte[] iv;
            byte[] array = Encoding.UTF8.GetBytes(value);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Key;
                iv = aes.IV;

                using (ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                {
                    using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream())
                    {
                        using (CryptoStream cryptoStream = new CryptoStream((System.IO.Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                        {
                            cryptoStream.Write(array, 0, array.Length);
                            cryptoStream.FlushFinalBlock();
                            return Convert.ToBase64String(iv.Concat(memoryStream.ToArray()).ToArray());
                        }
                    }
                }
            }
        }

        private static string Decrypt(string encryptedValue)
        {
            byte[] buffer = Convert.FromBase64String(encryptedValue);
            byte[] iv = new byte[16];
            byte[] array = new byte[buffer.Length - 16];

            Array.Copy(buffer, iv, 16);
            Array.Copy(buffer, 16, array, 0, buffer.Length - 16);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Key;
                aes.IV = iv;

                using (ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                {
                    using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream(array))
                    {
                        using (CryptoStream cryptoStream = new CryptoStream((System.IO.Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                        {
                            using (System.IO.StreamReader streamReader = new System.IO.StreamReader((System.IO.Stream)cryptoStream))
                            {
                                return streamReader.ReadToEnd();
                            }
                        }
                    }
                }
            }
        }
    }
}
