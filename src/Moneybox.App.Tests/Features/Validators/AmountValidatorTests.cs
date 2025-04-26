using Moneybox.App.Features.Validators;
using NUnit.Framework;

namespace Moneybox.App.Tests.Features.Validators
{
    public class AmountValidatorTests
    {
        [TestCase(-1, false, "Amount must be greater than zero")]
        [TestCase(0, false, "Amount must be greater than zero")]
        [TestCase(1, true, null)]
        [TestCase(1234.56, true, null)]

        public void Validate_WhenCalled_ReturnsExpectedResult(decimal amount, bool expectedIsValid, string expectedMessage)
        {
            var isValid = AmountValidator.Validate(amount, out var message);

            Assert.That(isValid, Is.EqualTo(expectedIsValid));
            Assert.That(message, Is.EqualTo(expectedMessage));
        }
    }
}