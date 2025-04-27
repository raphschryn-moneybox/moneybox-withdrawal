using Moneybox.App.DataAccess;
using Moneybox.App.Domain.Services;
using Moneybox.App.Features.Validators;
using System;

namespace Moneybox.App.Features
{
    public class WithdrawMoney
    {
        private readonly IAccountRepository _accountRepository;
        private readonly INotificationService _notificationService;

        public WithdrawMoney(IAccountRepository accountRepository, INotificationService notificationService)
        {
            _accountRepository = accountRepository;
            _notificationService = notificationService;
        }

        public void Execute(Guid fromAccountId, decimal amount)
        {
            if (!AmountValidator.Validate(amount, out var message))
            {
                throw new InvalidOperationException(message);
            }

            var from = _accountRepository.GetAccountById(fromAccountId);

            from.Withdraw(amount);

            _accountRepository.Update(from);

            if (from.IsLowBalance())
            {
                _notificationService.NotifyFundsLow(from.User.Email);
            }
        }
    }
}
