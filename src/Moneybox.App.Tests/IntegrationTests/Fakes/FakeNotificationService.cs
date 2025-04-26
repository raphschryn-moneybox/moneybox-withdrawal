using System.Collections.Generic;
using Moneybox.App.Domain.Services;

namespace Moneybox.App.Tests.IntegrationTests.Fakes
{
    public class FakeNotificationService : INotificationService
    {
        public List<string> FundsLowNotifications = new();
        public List<string> ApproachingPayInLimitNotifications = new();

        public void NotifyApproachingPayInLimit(string email)
        {
            ApproachingPayInLimitNotifications.Add(email);
        }

        public void NotifyFundsLow(string email)
        {
            FundsLowNotifications.Add(email);
        }
    }
}
