Feature: Legal Entity Updated
	In order know when a legal entity has been updated
	I want to be shown when its latest state


Scenario: A existing legal entity has been signed
	Given I have an existing unsigned legal entity
	When signed agreement event is triggered
	Then the legal entity state should be signed
