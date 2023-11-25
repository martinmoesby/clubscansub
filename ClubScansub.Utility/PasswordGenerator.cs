using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace ClubScansub.Utility
{
    public static class PasswordGenerator
    {

        public static string GetRandomAlphanumericString(int length)
        {
            const string alphanumericCharacters =
                "ABCDEFGHIJKMNPQRSTUVWXYZ" +
                "abcdefghijklmnopqrstuvwxyz" +
                "123456789";

            return getRandomString(length, alphanumericCharacters);
        }

        internal static string getRandomString(int length, IEnumerable<char> characterSet)
        {
            if (length < 0)
                throw new ArgumentException("length must not be negative", "length");
            if (length > int.MaxValue / 8) // 250 million chars ought to be enough for anybody
                throw new ArgumentException("length is too big", "length");
            if (characterSet == null)
                throw new ArgumentNullException("characterSet");
            var characterArray = characterSet.Distinct().ToArray();
            if (characterArray.Length == 0)
                throw new ArgumentException("characterSet must not be empty", "characterSet");

            var bytes = RandomNumberGenerator.GetBytes(length * 8);

            var result = new char[length];
            for (int i = 0; i < length; i++)
            {
                ulong value = BitConverter.ToUInt64(bytes, i * 8);
                result[i] = characterArray[value % (uint)characterArray.Length];
            }

            return new string(result);
        }
    }
}
