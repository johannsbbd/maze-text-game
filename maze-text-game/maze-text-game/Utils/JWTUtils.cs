using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace maze_text_game.Utils
{
    public static class JWTUtils
    {
        private const string delimiter = "[#]";
        public static bool VerifyToken(string token)
        {
            try
            {
                string decrypted = EncryptionHelper.Decrypt(token);                
                return decrypted.Contains(delimiter);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static string GetPlayerGuid(string token)
        {
            if (!VerifyToken(token)) return "N/a";

            string decrypted = EncryptionHelper.Decrypt(token);
            return decrypted.Split(delimiter)[1];
        }

        public static string GetPlayerName(string token)
        {
            if (!VerifyToken(token)) return "N/a";

            string decrypted = EncryptionHelper.Decrypt(token);
            return decrypted.Split(delimiter)[0];
        }

        public static string generateJWT(string playerName, string playerGuid)
        {
            string payload = playerName + delimiter + playerGuid;
            string encryptedPayload = EncryptionHelper.Encrypt(payload);
            return encryptedPayload;
        }
    }
}
