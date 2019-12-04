Feature: Legal Entity Updated
	In order know when a legal entity has been updated
	I want to be shown when its latest state

Scenario: A existing legal entity has been signed
	Given I have an existing unsigned, non levy legal entity
	When signed agreement event is triggered
	Then the legal entity should be signed

Scenario: A existing legal entity has recieved levy
	Given I have an existing unsigned, non levy legal entity
	When levy added event is triggered
	Then the legal entity should be marked as a levy entity

Scenario: A unknown legal entity has recieved levy
	Given I have a legal entity that is not in the database
	When levy added event is triggered
	Then an exception should be thrown

Scenario: A unknown legal entity has been signed
	Given I have a legal entity that is not in the database
	When signed agreement event is triggered
	Then an exception should be thrown
