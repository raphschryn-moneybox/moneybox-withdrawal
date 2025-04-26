using Moneybox.App.DataAccess;
using Moneybox.App.Domain.Services;
using Moneybox.App.Features.Validators;
using System;

namespace Moneybox.App.Features
{
    public class TransferMoney
    {
        private readonly IAccountRepository _accountRepository;
        private readonly INotificationService _notificationService;

        public TransferMoney(IAccountRepository accountRepository, INotificationService notificationService)
        {
            _accountRepository = accountRepository;
            _notificationService = notificationService;
        }

        public void Execute(Guid fromAccountId, Guid toAccountId, decimal amount)
        {
            if (!AmountValidator.Validate(amount, out var message))
            {
                throw new InvalidOperationException(message);
            }

            var from = _accountRepository.GetAccountById(fromAccountId);
            var to = _accountRepository.GetAccountById(toAccountId);

            from.TransferTo(to, amount);

            // TODO (out of scope): db transaction
            _accountRepository.Update(from);
            _accountRepository.Update(to);

            if (from.IsLowBalance())
            {
                _notificationService.NotifyFundsLow(from.User.Email);
            }

            if (to.IsApproachingPayInLimit())
            {
                _notificationService.NotifyApproachingPayInLimit(to.User.Email);
            }
        }
    }
}
