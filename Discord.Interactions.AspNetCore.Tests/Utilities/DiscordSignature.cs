using System.Text;

namespace TehGM.Discord.Interactions.Tests.Utilities
{
    // This class serves as an example of how to generate keys and signature for signing discord message
    // actual tests should use pre-generated values
    public static class DiscordSignature
    {
        public static (string publicKey, string signature) Sign(ulong timestamp, string messageBody)
        {
            Sodium.KeyPair keys = Sodium.PublicKeyAuth.GenerateKeyPair();
            string publicKey = Sodium.Utilities.BinaryToHex(keys.PublicKey);
            string msg = timestamp.ToString() + messageBody;
            byte[] signatureBytes = Sodium.PublicKeyAuth.SignDetached(Encoding.UTF8.GetBytes(msg), keys.PrivateKey);
            string signature = Sodium.Utilities.BinaryToHex(signatureBytes);
            return (publicKey, signature);
        }
    }
}
