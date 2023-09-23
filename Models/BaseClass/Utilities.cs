﻿using System.Net;
using System.Security.Cryptography;
using System.Text;
using static BaseClass.ReturnClass;
namespace BaseClass
{
    /// <summary>
    /// Summary description for Utilities
    /// </summary>
    public class Utilities
    {
        private static string _numbers = "123456789";
        ReturnBool rb = new();
        public Utilities()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        protected Random rGen = new Random();
        protected string[] strCharacters = { "A","B","C","D","E","F","G",
                    "H","I","J","K","L","M","N","O","P","Q","R","S","T","U","V","W","X","Y",
                    "Z","1","2","3","4","5","6","7","8","9","0","a","b","c","d","e","f","g","h",
                    "i","j","k","l","m","n","o","p","q","r","s","t","u","v","w","x","y","z"};

        #region Encryption and Hashing Algorithm
        /// <summary>
        /// Encrypt String using AES 256 Algorithm based on CBC and Zero Padding
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="encryptionKey"></param>
        /// <param name="inVect"></param>
        /// <returns></returns>
        public static string Aes256Encrypt(string plainText, string encryptionKey, string inVect)
        {
            byte[] encryptedBytes;
            using var aes = Aes.Create();
            aes.KeySize = 256;
            aes.BlockSize = 128;
            aes.Padding = PaddingMode.Zeros;
            aes.Mode = CipherMode.CBC;

            try
            {
                aes.Key = Encoding.ASCII.GetBytes(encryptionKey);
                aes.IV = Encoding.ASCII.GetBytes(inVect);

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using (MemoryStream memoryStream = new())
                {
                    using CryptoStream cryptoStream = new(memoryStream, encryptor, CryptoStreamMode.Write);
                    using (StreamWriter streamWriter = new(cryptoStream))
                    {
                        streamWriter.Write(plainText);
                    }
                    encryptedBytes = memoryStream.ToArray();
                }
                return (Convert.ToBase64String(encryptedBytes));
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Decrypt Cipher Text using AES 256 Algorithm based on CBC and Zero Padding
        /// </summary>
        /// <param name="cipherText"></param>
        /// <param name="encryptionKey"></param>
        /// <param name="inVect"></param>
        /// <returns>Plain Text</returns>
        public static string Aes256Decrypt(string cipherText, string encryptionKey, string inVect)
        {
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            string plainText = "";

            using var aes = Aes.Create();
            aes.KeySize = 256;
            aes.BlockSize = 128;
            aes.Padding = PaddingMode.Zeros;
            aes.Mode = CipherMode.CBC;
            try
            {
                aes.Key = Encoding.ASCII.GetBytes(encryptionKey);
                aes.IV = Encoding.ASCII.GetBytes(inVect);

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using (MemoryStream memoryStream = new(cipherBytes))
                {
                    using CryptoStream? csDecrypt = new(memoryStream, decryptor, CryptoStreamMode.Read);
                    using StreamReader? strmReader = new(csDecrypt);
                    plainText = strmReader.ReadToEnd();
                }
                return plainText;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Returns Hash of a input string
        /// </summary>
        /// <param name="input"></param>
        /// <param name="hashAlgorithm"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static string CreateHash(string input, HashingAlgorithmSupported hashAlgorithm)
        {
            string hash = hashAlgorithm switch
            {
                HashingAlgorithmSupported.Md5 => GenerateMd5Hash(input),
                HashingAlgorithmSupported.Sha256 => GenerateSHA256FromString(input),
                HashingAlgorithmSupported.Sha512 => GenerateSHA512FromString(input),
                _ => throw new NotImplementedException(),
            };
            return hash;
        }
        /// <summary>
        /// Generate Sha256 hash in lower hex format from input string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static string GenerateSHA256FromString(string input)
        {
            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(Encoding.Default.GetBytes(input));
            string hash = Convert.ToHexString(hashBytes);
            return hash.ToLower();
        }
        /// <summary>
        /// Generate Sha512 hash in lower hex format from input string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static string GenerateSHA512FromString(string input)
        {
            using var sha512 = SHA512.Create();
            var hashBytes = sha512.ComputeHash(Encoding.Default.GetBytes(input));
            string hash = Convert.ToHexString(hashBytes);
            return hash.ToLower();
        }
        /// <summary>
        /// Generates MD5 Hash in lower Hex for given string
        /// </summary>
        private static string GenerateMd5Hash(string input)
        {
            using var md5Hasher = MD5.Create();
            var hashBytes = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));
            string hashString = Convert.ToHexString(hashBytes);
            return hashString.ToLower();
        }
        #endregion

        #region Random Number and Password Generation Methods
        /// <summary>
        /// This method is used to generate random lowercase password with given length
        /// </summary>
        public string GenPassLowercase(int i)
        {
            int p = 0;
            string strPass = "";
            for (int x = 0; x <= i; x++)
            {
                p = rGen.Next(0, 35);
                strPass += strCharacters[p];
            }
            return strPass.ToLower();
        }

        /// <summary>
        /// This method is used to generate random uppercase password with given length
        /// </summary>
        public string GenPassWithCap(int i)
        {
            int p = 0;
            string strPass = "";
            for (int x = 0; x <= i; x++)
            {
                p = rGen.Next(0, 60);
                strPass += strCharacters[p];
            }
            return strPass.ToUpper();
        }
        /// <summary>
        /// This method is used to generate random string with given length
        /// </summary>
        public string GenRendomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public Int64 GenRendomNumber(int length)
        {
            Random rn = new Random();
            StringBuilder builder = new StringBuilder(6);
            for (var i = 0; i < length; i++)
            {
                builder.Append(_numbers[rn.Next(0, _numbers.Length)]);
            }

            string numberAsString = builder.ToString();
            int numberAsNumber = int.Parse(numberAsString);
            return numberAsNumber;
        }
        #endregion

        #region Number to Work Conversion
        public static string ConvertNumbersToWords(long number, LanguageSupported language)
        {
            string inWords = language switch
            {
                LanguageSupported.English => ConvertNumbertoEngish(number),
                LanguageSupported.Hindi => ConvertNumbertoWordsHindi(number),
                _ => throw new NotImplementedException(),
            };
            return inWords;
        }

        /// <summary>
        /// Numbers to Word - Hindi
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        private static string ConvertNumbertoWordsHindi(long number)
        {
            if (number == 0) return "शून्य";
            if (number < 0) return "minus " + ConvertNumbertoWordsHindi(Math.Abs(number));
            string words = "";
            if ((number / 1000000000) > 0)
            {
                words += ConvertNumbertoWordsHindi(number / 1000000000) + " अरब ";
                number %= 1000000000;
            }
            if ((number / 10000000) > 0)
            {
                words += ConvertNumbertoWordsHindi(number / 10000000) + " करोड़ ";
                number %= 10000000;
            }
            if ((number / 100000) > 0)
            {
                words += ConvertNumbertoWordsHindi(number / 100000) + " लाख ";
                number %= 100000;
            }
            if ((number / 1000) > 0)
            {
                words += ConvertNumbertoWordsHindi(number / 1000) + " हजार ";
                number %= 1000;
            }
            if ((number / 100) > 0)
            {
                words += ConvertNumbertoWordsHindi(number / 100) + " सौ ";
                number %= 100;
            }

            if (number > 0 && number < 100)
            {
                if (words != "") words += "";
                var unitsMap = new[]
                {
                "", "एक", "दो", "तीन", "चार", "पाँच", "छह", "सात", "आठ", "नौ", "दस",
                "ग्यारह", "बारह", "तेरह", "चौदह", "पन्द्रह", "सोलह", "सत्रह", "अठारह", "उन्नीस", "बीस",
                "इक्कीस", "बाईस", "तेईस", "चौबीस", "पच्चीस", "छब्बीस", "सत्ताईस", "अट्ठाईस", "उनतीस", "तीस",
                "इकतीस", "बत्तीस", "तैंतीस", "चौंतीस", "पैंतीस", "छत्तीस", "सैंतीस", "अड़तीस", "उनतालीस", "चालीस",
                "इकतालीस", "बयालीस", "तैंतालीस", "चौवालीस", "पैंतालीस", "छियालीस", "सैंतालीस", "अड़तालीस", "उनचास", "पचास",
                "इक्यावन", "बावन", "तिरेपन", "चौवन", "पचपन", "छप्पन", "सत्तावन", "अट्ठावन", "उनसठ", "साठ",
                "इकसठ", "बासठ", "तिरेसठ", "चौंसठ", "पैंसठ", "छियासठ", "सड़सठ", "अड़सठ", "उनहत्तर", "सत्तर",
                "इकहत्तर", "बहत्तर", "तिहत्तर", "चौहत्तर", "पचहत्तर", "छिहत्तर", "सतहत्तर", "अठहत्तर", "उनासी", "अस्सी",
                "इक्यासी", "बयासी", "तिरासी", "चौरासी", "पचासी", "छियासी", "सत्तासी", "अट्ठासी", "नवासी", "नब्बे",
                "इक्यानबे", "बानबे", "तिरानबे", "चौरानबे", "पंचानबे", "छियानबे", "सत्तानबे", "अट्ठानबे", "निन्यानबे"
            };

                if (number < 100) words += unitsMap[number];
                else
                {

                    if ((number % 10) > 0) words += " " + unitsMap[number % 10];
                }
            }
            return words;
        }
        /// <summary>
        /// Number to Words - English
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        private static string ConvertNumbertoEngish(long number)
        {
            string[] units = { "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten",
                           "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
            string[] tens = { "", "", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };
            try
            {
                if (number < 20)
                {
                    return units[number];
                }
                if (number < 100)
                {
                    return tens[number / 10] + ((number % 10 > 0) ? " " + ConvertNumbertoEngish(number % 10) : "");
                }
                if (number < 1000)
                {
                    return units[number / 100] + " Hundred" + ((number % 100 > 0) ? " And " + ConvertNumbertoEngish(number % 100) : "");
                }
                if (number < 100000)
                {
                    return ConvertNumbertoEngish(number / 1000) + " Thousand " + ((number % 1000 > 0) ? " " + ConvertNumbertoEngish(number % 1000) : "");
                }
                if (number < 10000000)
                {
                    return ConvertNumbertoEngish(number / 100000) + " Lakh " + ((number % 100000 > 0) ? " " + ConvertNumbertoEngish(number % 100000) : "");
                }
                if (number < 1000000000)
                {
                    return ConvertNumbertoEngish(number / 10000000) + " Crore " + ((number % 10000000 > 0) ? " " + ConvertNumbertoEngish(number % 10000000) : "");
                }
                return ConvertNumbertoEngish(number / 1000000000) + " Arab " + ((number % 1000000000 > 0) ? " " + ConvertNumbertoEngish(number % 1000000000) : "");
            }
            catch
            {
                throw;
            }
        }
        #endregion

        /// <summary>
        /// Get Client IP
        /// </summary>
        /// <param name="context"></param>
        /// <param name="allowForwarded"></param>
        /// <returns></returns>
        public static string GetRemoteIPAddress(HttpContext context, bool allowForwarded = true)
        {
            if (allowForwarded)
            {
                string header = context.Request.Headers["CF-Connecting-IP"].FirstOrDefault() ?? context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
                if (IPAddress.TryParse(header, out IPAddress ip))
                {
                    return ip.ToString(); ;
                }
            }
            return context.Connection.RemoteIpAddress.ToString();
        }

        /// <summary>
        /// Get Current Build from appsettings.json
        /// </summary>
        /// <returns></returns>
        public ReturnBool GetAppSettings(string rootNode, string firstChildrenNode)
        {
            var configurationBuilder = new ConfigurationBuilder();
            var path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            configurationBuilder.AddJsonFile(path, false);

            var root = configurationBuilder.Build();
            ReturnBool rb = new ();
            try
            {
                rb.message = root.GetSection(rootNode).GetSection(firstChildrenNode).Value;
                rb.status = true;
            }
            catch (Exception ex)
            {
                rb.error = ex.ToString();
            }
            return rb;
        }

        public ReturnBool GetAppSettings(string rootNode, string firstChildrenNode, string secondChildrenNode)
        {
            var configurationBuilder = new ConfigurationBuilder();
            var path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            configurationBuilder.AddJsonFile(path, false);

            var root = configurationBuilder.Build();
            ReturnBool rb = new();
            try
            {
                rb.message = root.GetSection(rootNode).GetSection(firstChildrenNode).GetSection(secondChildrenNode).Value;
                rb.status = true;
            }
            catch (Exception ex)
            {
                rb.error = ex.ToString();
            }
            return rb;
        }
        public string CreateHmacToken(string message, string secret)
        {
            secret ??= "";
            var encoding = new ASCIIEncoding();
            byte[] keyByte = encoding.GetBytes(secret);
            byte[] messageBytes = encoding.GetBytes(message);
            using var hmacsha256 = new HMACSHA256(keyByte);
            byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
            return Convert.ToBase64String(hashmessage);
        }
    }
}