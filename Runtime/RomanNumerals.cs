namespace DeepFreeze.Packages.Toolbox.Runtime
{
    public static class RomanNumerals
    {
        public static string ToRomanNumerals(this int number)
        {
            var roman = "";
 
            while (number > 0)
            {
                if (number >= 1000)
                {
                    roman += "M";
                    number -= 1000;
                }
                else if (number >= 900)
                {
                    roman += "CM";
                    number -= 900;
                }
                else if (number >= 500)
                {
                    roman += "D";
                    number -= 500;
                }
                else if (number >= 400)
                {
                    roman += "CD";
                    number -= 400;
                }
                else if (number >= 100)
                {
                    roman += "C";
                    number -= 100;
                }
                else if (number >= 90)
                {
                    roman += "XC";
                    number -= 90;
                }
                else if (number >= 50)
                {
                    roman += "L";
                    number -= 50;
                }
                else if (number >= 40)
                {
                    roman += "XL";
                    number -= 40;
                }
                else if (number > 10)
                {
                    roman += "X";
                    number -= 10;
                }
                else if (number <= 10)
                {
                    roman += GetRomanNumeral(number);
                    number = 0;
                }
            }
 
            return roman;
        }

        public static string GetRomanNumeral(int number)
        {
            string roman;
 
            switch (number)
            {
                case 1:
                    roman = "I";
                    break;
                case 2:
                    roman = "II";
                    break;
                case 3:
                    roman = "III";
                    break;
                case 4:
                    roman = "IV";
                    break;
                case 5:
                    roman = "V";
                    break;
                case 6:
                    roman = "VI";
                    break;
                case 7:
                    roman = "VII";
                    break;
                case 8:
                    roman = "VIII";
                    break;
                case 9:
                    roman = "IX";
                    break;
                case 10:
                    roman = "X";
                    break;
                default:
                    roman = number.ToString();
                    break;
            }
 
            return roman;
        }
    }
}
