using System;
using Moneybox.App.Features;
using Moneybox.App.Tests.IntegrationTests.Fakes;
using Moneybox.App.Tests.TestHelpers;
using NUnit.Framework;

namespace Moneybox.App.Tests.IntegrationTests
{
    public class TransferMoneyIntegrationTests
    {
        private FakeAccountRepository _accountRepository;
        private FakeNotificationService _notificationService;
        private TransferMoney _transferMoney;

        [SetUp]
        public void Setup()
        {
            _accountRepository = new FakeAccountRepository();
            _notificationService = new FakeNotificationService();
            _transferMoney = new TransferMoney(_accountRepository, _notificationService);
        }

        [Test]
        public void Execute_SuccessfulTransfer_UpdatesBalances()
        {
            var fromAccount = AccountTestHelper.CreateAccount();
            _accountRepository.Add(fromAccount);
            var toAccount = AccountTestHelper.CreateAccount(balance: 500m);
            _accountRepository.Add(toAccount);

            _transferMoney.Execute(fromAccount.Id, toAccount.Id, 200m);

            Assert.AreEqual(800m, fromAccount.Balance);
            Assert.AreEqual(700m, toAccount.Balance);
        }

        [Test]
        public void Execute_TransferCausingLowBalance_SendsFundsLowNotification()
        {
            var email = Guid.NewGuid().ToString() + "@test.com";
            var fromAccount = AccountTestHelper.CreateAccount(balance: 500m, email: email);
            _accountRepository.Add(fromAccount);
            var toAccount = AccountTestHelper.CreateAccount();
            _accountRepository.Add(toAccount);

            _transferMoney.Execute(fromAccount.Id, toAccount.Id, 200m);

            Assert.Contains(email, _notificationService.FundsLowNotifications);
        }

        [Test]
        public void Execute_TransferCausingApproachingPayInLimit_SendsApproachingLimitNotification()
        {
            var email = Guid.NewGuid().ToString() + "@test.com";
            var toAccount = AccountTestHelper.CreateAccount(paidIn: 3600m, email: email);
            _accountRepository.Add(toAccount);
            var fromAccount = AccountTestHelper.CreateAccount();
            _accountRepository.Add(fromAccount);

            _transferMoney.Execute(fromAccount.Id, toAccount.Id, 200m);

            Assert.Contains(email, _notificationService.ApproachingPayInLimitNotifications);
        }
    }
}
