namespace PremiersoftChallenge.BuildingBlocks.Validators
{
    public static class CpfValidator
    {
        public static bool IsCpfValid(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                return false;

            var digits = new string([.. cpf.Where(char.IsDigit)]);

            if (digits.Length != 11)
                return false;

            int sum = 0;
            int rest = 0;
            if (IsFirstDigitValid(sum, rest, digits))
            {
                return IsSecondDigitValid(sum, rest, digits);
            }

            return false;
        }

        private static bool IsFirstDigitValid(int sum, int rest, string digits)
        {
            for (int i = 0; i < 9; i++)
            {
                sum += (digits[i] - '0') * (10 - i);
            }

            rest = (sum * 10) % 11;
            if (rest == 10) rest = 0;

            int firstDigit = digits[9] - '0';
            if (rest != firstDigit)
                return false;

            return true;
        }

        private static bool IsSecondDigitValid(int sum, int rest, string digits)
        {
            sum = 0;
            for (int i = 0; i < 10; i++)
            {
                sum += (digits[i] - '0') * (11 - i);
            }

            rest = (sum * 10) % 11;
            if (rest == 10) rest = 0;

            int secondDigit = digits[10] - '0';
            if (rest != secondDigit)
                return false;

            return true;
        }
    }
}
