Feature: Apprenticeship Deleted
	In order to know when an apprenticeship has been deleted
	I want to be told that the reservation returns to a pending state


Scenario: Reservation status should be set to pending
	Given I have a confirmed reservation
	When a delete apprenticeship event is triggered
	Then the reservation status will be pending


Scenario: Non-Existing reservation should not cause re-queue
	Given I have a reservation that doesnt exist
	When a delete apprenticeship event is triggered
	Then the reservation does not cause a re-queue