using System;
using System.Collections.Generic;
using Moneybox.App.DataAccess;
using Moneybox.App.Domain;

namespace Moneybox.App.Tests.IntegrationTests.Fakes
{
    public class FakeAccountRepository : IAccountRepository
    {
        private readonly Dictionary<Guid, Account> _accounts = new();

        public Account GetAccountById(Guid accountId)
        {
            if (!_accounts.ContainsKey(accountId))
                throw new InvalidOperationException("Account not found.");

            return _accounts[accountId];
        }

        public void Update(Account account)
        {
            // Not implemented as the dictionary holds references
            // In real-life would use a dummy db context to update the account
        }

        public void Add(Account account)
        {
            _accounts[account.Id] = account;
        }
    }
}
