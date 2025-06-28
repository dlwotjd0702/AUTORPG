// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("SYX8tnVdla4ZoLk5OZE4U5aPC3cfrS4NHyIpJgWpZ6nYIi4uLiovLFYg79hpuXVwehTyI7Ew122i/bdDWgKUxtFLIsjLEh1MuAMNMda3l1CIjp3rggH/Yy/A5vEnG9oeRl7KSlIhgI4HpHStGqoOBn7hsi1lU3dAtqhkFP17LXUa1C9gAnl8Te0TuKYq7VKojI8GYEbv7F1CKyMWxpnvxyV1i0sKkQeV8AEkXVYmspREc3narS4gLx+tLiUtrS4uL+Rm8bdqMqXsxhan0pYswT3S6VJuhfca8L7DC6ftdilokoWUaN4f5GJItI79FESthQic2fuiNEeR9J3oLyjlBJho2IPiNYWOGPTOF/vF25d4Z6jUGSgJaXYBGqXjKtTbHi0sLi8u");
        private static int[] order = new int[] { 10,10,8,6,5,11,8,11,11,10,11,11,13,13,14 };
        private static int key = 47;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
