using System;

namespace Moneybox.App.Domain
{
    public class Account
    {
        public Guid Id { get; set; }

        public User User { get; set; }

        public decimal Balance { get; set; }

        public decimal Withdrawn { get; set; }

        public decimal PaidIn { get; set; }

        public decimal PayInLimit { get; private set; } = 4000m;

        #region methods
        public bool IsLowBalance() => Balance < 500;
        public bool IsApproachingPayInLimit() => PayInLimit - PaidIn < 500;
        public void TransferTo(Account to, decimal amount)
        {
            Withdraw(amount);
            to.PayIn(amount);
        }

        private bool CanWithdraw(decimal amount) => Balance - amount >= 0;

        private bool CanPayIn(decimal amount) => PaidIn + amount <= PayInLimit;

        private void Withdraw(decimal amount)
        {
            if (!CanWithdraw(amount))
                throw new InvalidOperationException("Insufficient funds to make transfer.");

            Balance -= amount;
            Withdrawn += amount;
        }

        private void PayIn(decimal amount)
        {
            if (!CanPayIn(amount))
                // TODO (out of scope): refactor to notify recipient
                // as this exception will only be seen by the sender
                throw new InvalidOperationException("Account pay in limit reached.");

            Balance += amount;
            PaidIn += amount;
        }
        #endregion
    }
}
