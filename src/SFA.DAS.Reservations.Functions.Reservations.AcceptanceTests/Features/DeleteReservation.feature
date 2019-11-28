Feature: Reservation deleted
	In order to know when a reservation has been deleted
	I want to be told when the reservation search index is updated


Scenario: Reservation search index should be updated on reservation deletion
	Given I have a reservation ready for deletion
	When a delete reservation event is triggered
	Then the reservation search index should be updated with the deleted reservation removed

Scenario: Employer should be notified on reservation deletion
	Given I have a reservation ready for creation
	When a delete reservation event is triggered
	Then the employer should be notified of the deleted reservation
