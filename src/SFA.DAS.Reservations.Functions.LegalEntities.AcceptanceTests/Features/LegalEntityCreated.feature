﻿Feature: LegalEntityCreated
	In order know when a legal entity has been created
	I want to be shown when its available


Scenario: A new legal entity has been created
	Given I have a legal entity that is new
	When added legal entity event is triggered
	Then the legal entity should be available
