Feature: Reservation confirmed
	In order to know when a reservation has been assigned	
	I want to be told when the reservation is used against an apprentice


Scenario: Reservation status should be set to confirmed
	Given I have a reservation
	When a confirm reservation event is triggered
	Then the reservation status will be confirmed
