using System.Text;

namespace Inventory.Core.Helper
{
    public static class StringHelper
    {
        public static string PasswordGenerate(int length)
        {
            const string validChars = "qwertyuiopasdfghjklzxcvbnmMNBVCXZLKJHGFDSAPOIUYTREWQ1234567890!@#$%^&*()";

            StringBuilder result = new();
            Random rand = new();

            for (int i = 0; i < length; i++)
            {
                int index = rand.Next(validChars.Length);
                result.Append(validChars[index]);
            }

            return result.ToString();
        }

        public static string CapitalizeFirstLetter(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            char[] characters = input.ToCharArray();
            characters[0] = char.ToUpper(characters[0]);

            return new string(characters);
        }
    }
}
