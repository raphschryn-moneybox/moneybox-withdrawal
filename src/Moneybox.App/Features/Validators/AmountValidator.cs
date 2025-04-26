
namespace Moneybox.App.Features.Validators
{
    public static class AmountValidator
    {
        public static bool Validate(decimal amount, out string message)
        {
            message = null;

            if (amount <= 0)
            {
                message = "Amount must be greater than zero";

                return false;
            }

            return true;
        }
    }
}
