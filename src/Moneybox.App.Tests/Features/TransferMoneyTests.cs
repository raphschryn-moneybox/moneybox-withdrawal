using Moneybox.App.DataAccess;
using Moneybox.App.Domain;
using Moneybox.App.Domain.Services;
using Moneybox.App.Features;
using Moneybox.App.Tests.TestHelpers;
using NSubstitute;
using NUnit.Framework;
using System;

namespace Moneybox.App.Tests.Features
{
    public class TransferMoneyTests
    {
        private IAccountRepository _accountRepository;
        private INotificationService _notificationService;
        private TransferMoney _transferMoney;
        private Account _fromAccount;
        private Account _toAccount;

        [SetUp]
        public void Setup()
        {
            _accountRepository = Substitute.For<IAccountRepository>();
            _notificationService = Substitute.For<INotificationService>();

            _transferMoney = new TransferMoney(_accountRepository, _notificationService);

            _fromAccount = AccountTestHelper.CreateAccount();
            _toAccount = AccountTestHelper.CreateAccount(balance: 500m);
        }

        [TestCase(-1)]
        [TestCase(0)]
        public void Execute_WhenAmountIsInvalid_ThrowsException(decimal invalidAmount)
        {
            Assert.Throws<InvalidOperationException>(() => _transferMoney.Execute(_fromAccount.Id, _toAccount.Id, invalidAmount));
        }

        [Test]
        public void Execute_WhenTransferIsValid_UpdatesAccounts()
        {
            _accountRepository.GetAccountById(_fromAccount.Id).Returns(_fromAccount);
            _accountRepository.GetAccountById(_toAccount.Id).Returns(_toAccount);

            _transferMoney.Execute(_fromAccount.Id, _toAccount.Id, 200m);

            _accountRepository.Received(1).Update(_fromAccount);
            _accountRepository.Received(1).Update(_toAccount);
        }

        [Test]
        public void Execute_WhenLowBalance_NotifiesUser()
        {
            var fromAccount = AccountTestHelper.CreateAccount(balance: 600m);

            _accountRepository.GetAccountById(fromAccount.Id).Returns(fromAccount);
            _accountRepository.GetAccountById(_toAccount.Id).Returns(_toAccount);

            _transferMoney.Execute(fromAccount.Id, _toAccount.Id, 200m);

            _notificationService.Received(1).NotifyFundsLow(fromAccount.User.Email);
        }

        [Test]
        public void Execute_WhenApproachingPayInLimit_NotifiesReceiver()
        {
            var toAccount = AccountTestHelper.CreateAccount(paidIn: 3600m);

            _accountRepository.GetAccountById(_fromAccount.Id).Returns(_fromAccount);
            _accountRepository.GetAccountById(toAccount.Id).Returns(toAccount);

            _transferMoney.Execute(_fromAccount.Id, toAccount.Id, 200m);

            _notificationService.Received(1).NotifyApproachingPayInLimit(toAccount.User.Email);
        }
    }
}