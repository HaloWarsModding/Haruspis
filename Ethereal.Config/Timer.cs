using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;

using Ethereal.Logging;

namespace Ethereal.Config
{
    public class ETHData
    {
        #region Properties
        public Timers Data { get; set; } = new Timers();

        public class Timers
        {
            public TimeSpan PlayTime { get; set; } = TimeSpan.Zero;
            public DateTime LastPlayed { get; set; } = DateTime.MinValue;
        }
        #endregion

        private readonly string encryptionKey = "NtipEx0GJR9x6vJUT7pLsJCD8riFIu8X"; // Yes I'm keeping it here, I don't want kids to change their playtime to "flex". But if you go to the length of cloning the repo and checking the encryption yourself..I think you deserve it.

        public void ToFile(string path)
        {
            try
            {
                string serializedData = SerializeData(this);
                string encryptedData = EncryptString(serializedData, encryptionKey);

                using (StreamWriter streamWriter = new(path))
                {
                    streamWriter.Write(encryptedData);
                }

                Logger.Log(LogLevel.Information, "Data file successfully written to: " + path);
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, "Error writing data file: " + ex.Message);
            }
        }

        public void FromFile(string path)
        {
            if (!DataFileExists(path))
            {
                Logger.Log(LogLevel.Error, "Data file does not exist: " + path);
                return;
            }

            try
            {
                string encryptedData;
                using (StreamReader streamReader = new(path))
                {
                    encryptedData = streamReader.ReadToEnd();
                }

                string decryptedData = DecryptString(encryptedData, encryptionKey);
                ETHData deserializedObject = DeserializeData<ETHData>(decryptedData);

                if (deserializedObject != null)
                {
                    Data = deserializedObject.Data;
                    Logger.Log(LogLevel.Information, $"Data file successfully read from: {path}");
                    Logger.Log(LogLevel.Debug, $"PlayTime: {Data.PlayTime}, LastPlayed: {Data.LastPlayed}");
                }
                else
                {
                    Logger.Log(LogLevel.Error, "Failed to deserialize data object from file: " + path);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, "Error reading data file: " + ex.Message);
            }
        }

        private static bool DataFileExists(string path)
        {
            return File.Exists(path);
        }

        private static string SerializeData<T>(T data)
        {
            XmlSerializer xmlSerializer = new(typeof(T));
            using StringWriter textWriter = new();
            xmlSerializer.Serialize(textWriter, data);
            return textWriter.ToString();
        }

        private static T DeserializeData<T>(string xmlData)
        {
            XmlSerializer xmlSerializer = new(typeof(T));
            using StringReader textReader = new(xmlData);
            return (T)xmlSerializer.Deserialize(textReader)!;
        }

        private static string EncryptString(string plainText, string key)
        {
            byte[] encryptedData;
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(key);
                aesAlg.IV = new byte[16];
                aesAlg.Mode = CipherMode.CBC;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                using MemoryStream msEncrypt = new();
                using CryptoStream csEncrypt = new(msEncrypt, encryptor, CryptoStreamMode.Write);
                using (StreamWriter swEncrypt = new(csEncrypt))
                {
                    swEncrypt.Write(plainText);
                }
                encryptedData = msEncrypt.ToArray();
            }
            return Convert.ToBase64String(encryptedData);
        }

        private static string DecryptString(string encryptedText, string key)
        {
            byte[] cipherText = Convert.FromBase64String(encryptedText);
            string? plaintext = null;
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(key);
                aesAlg.IV = new byte[16];
                aesAlg.Mode = CipherMode.CBC;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                using MemoryStream msDecrypt = new(cipherText);
                using CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read);
                using StreamReader srDecrypt = new(csDecrypt);
                plaintext = srDecrypt.ReadToEnd();
            }
            return plaintext;
        }
    }
}
