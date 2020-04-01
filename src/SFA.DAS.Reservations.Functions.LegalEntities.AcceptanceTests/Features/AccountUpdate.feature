Feature: Account Updated
  In order know when an account has been updated
  I want to be process events for when the state changes

  Scenario: A new account added
    Given I receive an Account created event
    Then the account is created
    
  Scenario: Duplicate Account message received
    Given I have a non levy account    
    When I receive an Account created event
    Then the account is not duplicated
      
  Scenario: Account name updated
    Given I have a non levy account
    When I receive an Account name updated event
    Then the account name is updated
    
  Scenario: Account levy status updated
    Given I have a non levy account
    When levy added event is triggered
    Then the account should be marked as a levy