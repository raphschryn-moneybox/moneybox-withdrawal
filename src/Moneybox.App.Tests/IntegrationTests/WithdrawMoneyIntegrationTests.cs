using Moneybox.App.Domain;
using Moneybox.App.Features;
using Moneybox.App.Tests.IntegrationTests.Fakes;
using Moneybox.App.Tests.TestHelpers;
using NUnit.Framework;
using System;

namespace Moneybox.App.Tests.IntegrationTests
{
    public class WithdrawMoneyIntegrationTests
    {
        private FakeAccountRepository _accountRepository;
        private FakeNotificationService _notificationService;
        private WithdrawMoney _withdrawMoney;

        [SetUp]
        public void Setup()
        {
            _accountRepository = new FakeAccountRepository();
            _notificationService = new FakeNotificationService();

            _withdrawMoney = new WithdrawMoney(_accountRepository, _notificationService);           
        }

        [Test]
        public void Execute_SuccessfulWithdrawal_UpdatesBalance()
        {
            var fromAccount = AccountTestHelper.CreateAccount();
            _accountRepository.Add(fromAccount);

            _withdrawMoney.Execute(fromAccount.Id, 300m);

            var updatedAccount = _accountRepository.GetAccountById(fromAccount.Id);

            Assert.AreEqual(700m, updatedAccount.Balance);
        }

        [Test]
        public void Execute_WithdrawalCausingLowBalance_SendsFundsLowNotification()
        {
            var email = Guid.NewGuid().ToString() + "@test.com";
            var fromAccount = AccountTestHelper.CreateAccount(balance: 600m, email: email);
            _accountRepository.Add(fromAccount);

            _withdrawMoney.Execute(fromAccount.Id, 200m);

            Assert.Contains(email, _notificationService.FundsLowNotifications);
        }
    }
}
