using Moneybox.App.Domain;
using NUnit.Framework;
using System;

namespace Moneybox.App.Tests.Domain
{
    public class AccountTests
    {
        private Account _from;
        private Account _to;

        [SetUp]
        public void Setup()
        {
            _from = new Account
            {
                Id = Guid.NewGuid(),
                Balance = 1000m,
                Withdrawn = 0m,
                PaidIn = 0m
            };

            _to = new Account
            {
                Id = Guid.NewGuid(),
                Balance = 500m,
                Withdrawn = 0m,
                PaidIn = 0m
            };
        }

        [Test]
        public void TransferTo_ShouldTransferFunds_WhenSufficientBalanceAndWithinPayInLimit()
        {
            var amount = 200m;

            _from.TransferTo(_to, amount);

            Assert.AreEqual(800m, _from.Balance);
            Assert.AreEqual(200m, _from.Withdrawn);
            Assert.AreEqual(700m, _to.Balance);
            Assert.AreEqual(200m, _to.PaidIn);
        }

        [Test]
        public void TransferTo_ShouldThrow_WhenInsufficientFunds()
        {
            var amount = 2000m;

            var ex = Assert.Throws<InvalidOperationException>(() => _from.TransferTo(_to, amount));
            Assert.That(ex.Message, Is.EqualTo("Insufficient funds to make transfer."));
        }

        [Test]
        public void TransferTo_ShouldThrow_WhenRecipientExceedsPayInLimit()
        {
            _to.PaidIn = 3900m;
            var amount = 200m;

            var ex = Assert.Throws<InvalidOperationException>(() => _from.TransferTo(_to, amount));
            Assert.That(ex.Message, Is.EqualTo("Account pay in limit reached."));
        }

        [Test]
        public void IsLowBalance_ShouldReturnTrue_WhenBalanceBelow500()
        {
            _from.Balance = 499m;

            var result = _from.IsLowBalance();

            Assert.IsTrue(result);
        }

        [Test]
        public void IsLowBalance_ShouldReturnFalse_WhenBalanceAbove500()
        {
            _from.Balance = 501m;

            var result = _from.IsLowBalance();

            Assert.IsFalse(result);
        }

        [Test]
        public void IsApproachingPayInLimit_ShouldReturnTrue_WhenLessThan500LeftToPayIn()
        {
            _to.PaidIn = 3600m;

            var result = _to.IsApproachingPayInLimit();

            Assert.IsTrue(result);
        }

        [Test]
        public void IsApproachingPayInLimit_ShouldReturnFalse_WhenMoreThan500LeftToPayIn()
        {
            _to.PaidIn = 2000m;

            var result = _to.IsApproachingPayInLimit();

            Assert.IsFalse(result);
        }
    }
}