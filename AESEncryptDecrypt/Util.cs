using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;

namespace AESEncryptDecrypt
{
    internal class Util
    {
        //AES
        #region AES Variables
        const int theKeySize = 256;
        const CipherMode cipherMode = CipherMode.CBC;
        #endregion

        /// <summary>
        /// Convert string data to encrypted base64 string using AES 256bit CBC cipher mode
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <param name="IV"></param>
        /// <returns></returns>
        public string EncryptToBase64(string data, string key, string IV)
        {
            //store encrypted byte array
            byte[] encrypted;

            //params
            byte[] theKey = Encoding.UTF8.GetBytes(key);
            byte[] theIV = Encoding.UTF8.GetBytes(IV);
            byte[] theData = Encoding.UTF8.GetBytes(data);

            //Compresses the supplied data with GZIP, optional
            theData = GZIPCompress(theData);

            //Encrypts the data
            using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
            {
                //assign AES key and mode, pass key(32) and IV(16)
                aes.KeySize = theKeySize;
                aes.Mode = cipherMode;
                aes.Key = theKey;
                aes.IV = theIV;

                //Create encryptor
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                //Create encrypted stream
                using MemoryStream memoryStream = new MemoryStream();
                using CryptoStream cryptoEncryptStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
                cryptoEncryptStream.Write(theData, 0, theData.Length);
                cryptoEncryptStream.FlushFinalBlock();

                //set encrypted byte array
                encrypted = memoryStream.ToArray();
            }

            //Returns base64 string of encrypted byte
            return Convert.ToBase64String(encrypted);
        }


        public string DecryptData(string data, string key, string IV)
        {
            //Store Decrypts data
            string decryptedData = null;

            //Convert from Base64 to byte array
            byte[] encryptedData = Convert.FromBase64String(data);
            byte[] theKey = Encoding.UTF8.GetBytes(key);
            byte[] theIV = Encoding.UTF8.GetBytes(IV);


            using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
            {
                //assign AES mode, key and IV key
                aes.KeySize = theKeySize;
                aes.Mode = cipherMode;
                aes.Key = theKey;
                aes.IV = theIV;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using (MemoryStream Memory = new MemoryStream(encryptedData))
                {
                    using (CryptoStream Decryptor = new CryptoStream(Memory, decryptor, CryptoStreamMode.Read))
                    {
                        using (MemoryStream tempMemory = new MemoryStream())
                        {
                            byte[] Buffer = new byte[1024];
                            Int32 readBytes = 0;
                            while ((readBytes = Decryptor.Read(Buffer, 0, Buffer.Length)) > 0)
                            {
                                tempMemory.Write(Buffer, 0, readBytes);
                            }
                            //if you comment out GZip compression make sure to comment this too
                            byte[] unzipped = GZIPDecompress(tempMemory.ToArray());
                            decryptedData = Encoding.UTF8.GetString(unzipped);
                        }
                    }
                }
            }

            return decryptedData;
        }

        #region GZip Compress and Decompress
        /// <summary>
        /// GZip a byte array
        /// </summary>
        /// <param name="unzipedData"></param>
        /// <returns></returns>
        private byte[] GZIPCompress(byte[] unzipedData)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (GZipStream zipStream = new GZipStream(memoryStream, CompressionMode.Compress))
                {
                    zipStream.Write(unzipedData, 0, unzipedData.Length);
                }

                return memoryStream.ToArray();
            }
        }

        /// <summary>
        /// Undo Gzip byte array
        /// </summary>
        /// <param name="gzippedData"></param>
        /// <returns></returns>
        private byte[] GZIPDecompress(byte[] gzippedData)
        {
            using (MemoryStream inputStream = new MemoryStream(gzippedData))
            {
                using (GZipStream zipStream = new GZipStream(inputStream, CompressionMode.Decompress))
                {
                    using (MemoryStream outputStream = new MemoryStream())
                    {
                        zipStream.CopyTo(outputStream);
                        return outputStream.ToArray();
                    }
                }
            }
        }

        #endregion
    }
}
