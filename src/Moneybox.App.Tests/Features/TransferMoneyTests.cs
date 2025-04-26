using Moneybox.App.DataAccess;
using Moneybox.App.Domain;
using Moneybox.App.Domain.Services;
using Moneybox.App.Features;
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

            _fromAccount = new Account
            {
                Id = Guid.NewGuid(),
                Balance = 1000m,
                Withdrawn = 0m,
                PaidIn = 0m,
                User = new User { Email = "from@test.com" }
            };

            _toAccount = new Account
            {
                Id = Guid.NewGuid(),
                Balance = 500m,
                Withdrawn = 0m,
                PaidIn = 0m,
                User = new User { Email = "to@test.com" }
            };
        }

        [Test]
        public void Execute_WhenAmountIsInvalid_ThrowsException()
        {
            Assert.Throws<InvalidOperationException>(() => _transferMoney.Execute(_fromAccount.Id, _toAccount.Id, -10m));
        }

        [Test]
        public void Execute_WhenTransferIsValid_UpdatesAccounts()
        {
            // Arrange
            _accountRepository.GetAccountById(_fromAccount.Id).Returns(_fromAccount);
            _accountRepository.GetAccountById(_toAccount.Id).Returns(_toAccount);

            // Act
            _transferMoney.Execute(_fromAccount.Id, _toAccount.Id, 200m);

            // Assert
            _accountRepository.Received(1).Update(_fromAccount);
            _accountRepository.Received(1).Update(_toAccount);
        }

        [Test]
        public void Execute_WhenLowBalance_NotifiesUser()
        {
            // Arrange
            _fromAccount.Balance = 600m;
            _accountRepository.GetAccountById(_fromAccount.Id).Returns(_fromAccount);
            _accountRepository.GetAccountById(_toAccount.Id).Returns(_toAccount);

            // Act
            _transferMoney.Execute(_fromAccount.Id, _toAccount.Id, 200m);

            // Assert
            _notificationService.Received(1).NotifyFundsLow(_fromAccount.User.Email);
        }

        [Test]
        public void Execute_WhenApproachingPayInLimit_NotifiesReceiver()
        {
            // Arrange
            _toAccount.PaidIn = 3600m;
            _accountRepository.GetAccountById(_fromAccount.Id).Returns(_fromAccount);
            _accountRepository.GetAccountById(_toAccount.Id).Returns(_toAccount);

            // Act
            _transferMoney.Execute(_fromAccount.Id, _toAccount.Id, 200m);

            // Assert
            _notificationService.Received(1).NotifyApproachingPayInLimit(_toAccount.User.Email);
        }
    }
}