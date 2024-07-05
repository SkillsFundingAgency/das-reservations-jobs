using System;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.EmployerFinance.Messages.Events;
using SFA.DAS.Reservations.Data;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;
using SFA.DAS.Reservations.Domain.Accounts;
using TechTalk.SpecFlow;

namespace SFA.DAS.Reservations.Functions.LegalEntities.AcceptanceTests.Steps;

[Binding]
public class AccountSteps :StepsBase
{
    public AccountSteps(TestServiceProvider serviceProvider, TestData testData) : base(serviceProvider, testData)
    {
    }
        
    [Given(@"I have a non levy account")]
    public void WhenIHaveANonLevyAccount()
    {
        var dbContext = Services.GetService<ReservationsDataContext>();
            
        var account = dbContext.Accounts.SingleOrDefault(e => e.Id.Equals(TestData.NonLevyAccount.Id));

        if (account == null)
        {
            dbContext.Accounts.Add(TestData.NonLevyAccount);
            dbContext.SaveChanges();
        }
    }
        
    [Given(@"I receive an Account created event")]
    [When(@"I receive an Account created event")]
    public void GivenIReceiveAnAccountCreatedEvent()
    {
        var handler = Services.GetService<IAddAccountHandler>();
            
        try
        {
            handler.Handle(new CreatedAccountEvent
            {
                AccountId = TestData.NonLevyAccount.Id,
                Name = TestData.NonLevyAccount.Name
            }).Wait();
        }
        catch (Exception e)
        {
            TestData.Exception = e;
        }
            
    }
        
    [When(@"levy added event is triggered")]
    public void WhenLevyAddedEventIsTriggered()
    {
        var handler = Services.GetService<ILevyAddedToAccountHandler>();

        try
        {
            handler.Handle(new LevyAddedToAccount
            {
                AccountId = TestData.NonLevyAccount.Id
            }).Wait();
        }
        catch (Exception e)
        {
            TestData.Exception = e;
        }
    }
        
    [When(@"I receive an Account name updated event")]
    public void WhenIReceiveAnAccountNameUpdatedEvent()
    {
        var handler = Services.GetService<IAccountNameUpdatedHandler>();

        TestData.NewAccountName = "My New Account Name";
            
        try
        {
            handler.Handle(new ChangedAccountNameEvent
            {
                AccountId = TestData.NonLevyAccount.Id,
                CurrentName = TestData.NewAccountName
            }).Wait();
        }
        catch (Exception e)
        {
            TestData.Exception = e;
        }
    }

    [Then(@"the account should be marked as a levy")]
    public void ThenTheAccountShouldBeMarkedAsLevy()
    {
        var dbContext = Services.GetService<ReservationsDataContext>();
        var account = dbContext.Accounts.SingleOrDefault(acc =>
            acc.Id.Equals(TestData.NonLevyAccount.Id));

        account.Should().NotBeNull();
        account.IsLevy.Should().BeTrue();
    }

    [Then(@"the account is created")]
    public void ThenTheAccountIsCreated()
    {
        var dbContext = Services.GetService<ReservationsDataContext>();
        var account = dbContext.Accounts.SingleOrDefault(acc =>
            acc.Id.Equals(TestData.NonLevyAccount.Id));

        account.Should().NotBeNull();
    }
        
    [Then(@"the account name is updated")]
    public void ThenTheAccountNameIsUpdated()
    {
        var dbContext = Services.GetService<ReservationsDataContext>();
        var account = dbContext.Accounts.SingleOrDefault(acc =>
            acc.Id.Equals(TestData.NonLevyAccount.Id));
            
        account.Should().NotBeNull();
        account.Name.Should().Be(TestData.NewAccountName);
    }

    [Then(@"the account is not duplicated")]
    public void ThenTheAccountIsNotDuplicated()
    {
        var dbContext = Services.GetService<ReservationsDataContext>();
        var numberOfAccounts = dbContext.Accounts.Count(c => c.Id.Equals(TestData.NonLevyAccount.Id));

        numberOfAccounts.Should().Be(1);
    }
}