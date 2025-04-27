using Moneybox.App.Domain;
using Moneybox.App.Tests.TestHelpers;
using NUnit.Framework;
using System;

namespace Moneybox.App.Tests.Domain
{
    public class AccountTests
    {
        private Account _fromAccount;
        private Account _toAccount;

        [SetUp]
        public void Setup()
        {
            _fromAccount = AccountTestHelper.CreateAccount();
            _toAccount = AccountTestHelper.CreateAccount(balance: 500m);
        }

        [Test]
        public void TransferTo_ShouldTransferFunds_WhenSufficientBalanceAndWithinPayInLimit()
        {
            var amount = 200m;

            _fromAccount.TransferTo(_toAccount, amount);

            Assert.AreEqual(800m, _fromAccount.Balance);
            Assert.AreEqual(200m, _fromAccount.Withdrawn);
            Assert.AreEqual(700m, _toAccount.Balance);
            Assert.AreEqual(200m, _toAccount.PaidIn);
        }

        [Test]
        public void TransferTo_ShouldThrow_WhenInsufficientFunds()
        {
            var amount = 2000m;

            var ex = Assert.Throws<InvalidOperationException>(() => _fromAccount.TransferTo(_toAccount, amount));
            
            Assert.That(ex.Message, Is.EqualTo("Insufficient funds to make transfer."));
        }

        [Test]
        public void TransferTo_ShouldThrow_WhenRecipientExceedsPayInLimit()
        {
            var toAccount = AccountTestHelper.CreateAccount(paidIn: 3900m);
            var amount = 200m;

            var ex = Assert.Throws<InvalidOperationException>(() => _fromAccount.TransferTo(toAccount, amount));
            
            Assert.That(ex.Message, Is.EqualTo("Account pay in limit reached."));
        }

        [Test]
        public void IsLowBalance_ShouldReturnTrue_WhenBalanceBelow500()
        {
            var fromAccount = AccountTestHelper.CreateAccount(balance: 499m);

            var result = fromAccount.IsLowBalance();

            Assert.IsTrue(result);
        }

        [Test]
        public void IsLowBalance_ShouldReturnFalse_WhenBalanceAbove500()
        {
            var fromAccount = AccountTestHelper.CreateAccount(balance: 501m);

            var result = fromAccount.IsLowBalance();

            Assert.IsFalse(result);
        }

        [Test]
        public void IsApproachingPayInLimit_ShouldReturnTrue_WhenLessThan500LeftToPayIn()
        {
            var toAccount = AccountTestHelper.CreateAccount(paidIn: 3600m);

            var result = toAccount.IsApproachingPayInLimit();

            Assert.IsTrue(result);
        }

        [Test]
        public void IsApproachingPayInLimit_ShouldReturnFalse_WhenMoreThan500LeftToPayIn()
        {
            var toAccount = AccountTestHelper.CreateAccount(paidIn: 2000m);

            var result = toAccount.IsApproachingPayInLimit();

            Assert.IsFalse(result);
        }
    }
}