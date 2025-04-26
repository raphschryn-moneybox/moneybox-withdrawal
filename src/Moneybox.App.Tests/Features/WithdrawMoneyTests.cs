using Moneybox.App.DataAccess;
using Moneybox.App.Domain.Services;
using Moneybox.App.Features;
using NUnit.Framework;
using NSubstitute;
using System;
using Moneybox.App.Domain;
using Moneybox.App.Tests.TestHelpers;

namespace Moneybox.App.Tests.Features
{
    public class WithdrawMoneyTests
    {
        private IAccountRepository _accountRepository;
        private INotificationService _notificationService;
        private WithdrawMoney _withdrawMoney;
        private Account _fromAccount;

        [SetUp]
        public void Setup()
        {
            _accountRepository = Substitute.For<IAccountRepository>();
            _notificationService = Substitute.For<INotificationService>();
            _withdrawMoney = new WithdrawMoney(_accountRepository, _notificationService);

            _fromAccount = AccountTestHelper.CreateAccount();
        }

        [TestCase(-1)]
        [TestCase(0)]
        public void Execute_WhenAmountIsInvalid_ThrowsException(decimal invalidAmount)
        {
            Assert.Throws<InvalidOperationException>(() => _withdrawMoney.Execute(_fromAccount.Id, invalidAmount));
        }

        [Test]
        public void Execute_WithValidAmount_UpdatesAccount()
        {
            _accountRepository.GetAccountById(_fromAccount.Id).Returns(_fromAccount);

            _withdrawMoney.Execute(_fromAccount.Id, 100m);

            _accountRepository.Received(1).Update(_fromAccount);
        }

        [Test]
        public void Execute_WithWithdrawalLeadingToLowBalance_NotifiesUser()
        {
            var fromAccount = AccountTestHelper.CreateAccount(balance: 600m);

            _accountRepository.GetAccountById(fromAccount.Id).Returns(fromAccount);

            _withdrawMoney.Execute(fromAccount.Id, 200m);

            _notificationService.Received(1).NotifyFundsLow(fromAccount.User.Email);
        }

        [Test]
        public void Execute_WithWithdrawalMaintainingGoodBalance_DoesNotNotifyUser()
        {
             var fromAccount = AccountTestHelper.CreateAccount(balance: 1000m);

            _accountRepository.GetAccountById(fromAccount.Id).Returns(fromAccount);

            _withdrawMoney.Execute(fromAccount.Id, 100m);

            _notificationService.DidNotReceive().NotifyFundsLow("Account pay in limit reached.");
        }
    }
}
