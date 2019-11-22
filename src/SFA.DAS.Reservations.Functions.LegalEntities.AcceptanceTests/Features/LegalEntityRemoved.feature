﻿Feature: Legal Entity Removed
	In order know when a legal entity has been removed
	I want to be shown when its no longer available


Scenario: An existing legal entity has been removed
	Given I have an existing unsigned legal entity
	When removed legal entity event is triggered
	Then the legal entity should no longer be available
