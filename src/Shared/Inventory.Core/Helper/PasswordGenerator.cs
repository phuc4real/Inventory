using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Core.Helper
{
    public static class PasswordGenerator
    {
        public static string Generate(int length)
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
    }
}
