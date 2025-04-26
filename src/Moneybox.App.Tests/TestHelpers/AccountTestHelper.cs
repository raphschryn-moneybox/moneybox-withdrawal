using System;
using Moneybox.App.Domain;

namespace Moneybox.App.Tests.TestHelpers
{
    public static class AccountTestHelper
    {
        public static Account CreateAccount(
            Guid? id = null,
            string email = "default@test.com",
            decimal balance = 1000m,
            decimal withdrawn = 0m,
            decimal paidIn = 0m,
            decimal payInLimit = 4000m)
        {
            var account = new Account(
                id ?? Guid.NewGuid(),
                new User { Email = email },
                balance,
                withdrawn,
                paidIn,
                payInLimit)
            ;

            return account;
        }
    }
}
