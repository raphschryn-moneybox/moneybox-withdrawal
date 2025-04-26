# Moneybox Money Withdrawal

## Code Explanation

- **PayInLimit Property:**  
  In `Domain/Account.cs`, the `PayInLimit` property has been changed from a `const` to a standard variable with a private setter. This allows each `Account` to have different pay-in limits if needed. Its default value is set to the previous constant value.

- **Encapsulated Balance Changes:**  
  Methods to handle balance changes (such as `Withdraw`, `PayIn`, and related validations) have been implemented directly within the `Account` domain model.  
  This allows sensitive properties like `Balance` to have private setters, preserving the integrity of account state.

- **Notification Timing:**  
  Notifications (e.g., low funds, approaching pay-in limit) are now triggered *after* the database update, ensuring customers are only notified once their account state has successfully changed.  
  This prevents confusing notifications if a database error occurs.

- **Amount Validation:**  
  Validation has been added for the `amount` parameter in both service methods (`TransferMoney` and `WithdrawMoney`) to reject invalid transactions early.

- **Mocking Strategy:**  
  Unit tests use **NSubstitute** instead of **Moq** due to recent controversies around Moq collecting telemetry data.  

- **Error Handling for Missing Accounts:**  
  The code assumes that `GetAccountById(Guid accountId)` will throw an exception if the account does not exist.  
  If this is not the case, explicit null checks should be added.

## Nice to Have (Out of Scope)

- If a money transfer fails because the recipient’s account would exceed its pay-in limit, currently **only the sender is notified** via an exception.  
  Ideally, the recipient should also be notified.

- Transfers should be wrapped in a **database transaction** to prevent money from disappearing (or magically being created) if an error occurs midway.

- **Input Validation:**  
  Integrating `FluentValidation` for cleaner, reusable validation rules would improve input handling.

- **Framework Upgrade:**  
  Upgrade the solution from **.NET 5** to **.NET 9**.

- **Account Reset:**  
  Implement scheduled resets (daily, weekly, or monthly) for `PaidIn` and `Withdrawn` properties.

- **Overdraft Functionality:**  
  Extend the `Account` model to support controlled overdrafts.

- **Further Integration Tests:**  
  Add integration tests covering sequences of operations, such as **withdraw then transfer**, to verify consistent behavior over multiple actions.


# Task

The solution contains a .NET core library (Moneybox.App) which is structured into the following 3 folders:

* Domain - this contains the domain models for a user and an account, and a notification service.
* Features - this contains two operations, one which is implemented (transfer money) and another which isn't (withdraw money)
* DataAccess - this contains a repository for retrieving and saving an account (and the nested user it belongs to)

## The task

The task is to implement a money withdrawal in the WithdrawMoney.Execute(...) method in the features folder. For consistency, the logic should be the same as the TransferMoney.Execute(...) method i.e. notifications for low funds and exceptions where the operation is not possible. 

As part of this process however, you should look to refactor some of the code in the TransferMoney.Execute(...) method into the domain models, and make these models less susceptible to misuse. We're looking to make our domain models rich in behaviour and much more than just plain old objects, however we don't want any data persistance operations (i.e. data access repositories) to bleed into our domain. This should simplify the task of implementing WithdrawMoney.Execute(...).

## Guidelines

* The test should take about an hour to complete, although there is no strict time limit
* You should fork or copy this repository into your own public repository (Github, BitBucket etc.) before you do your work
* Your solution must build and any tests must pass
* You should not alter the notification service or the the account repository interfaces
* You may add unit/integration tests using a test framework (and/or mocking framework) of your choice
* You may edit this README.md if you want to give more details around your work (e.g. why you have done something a particular way, or anything else you would look to do but didn't have time)

Once you have completed test, zip up your solution, excluding any build artifacts to reduce the size, and email it back to our recruitment team.

Good luck!
