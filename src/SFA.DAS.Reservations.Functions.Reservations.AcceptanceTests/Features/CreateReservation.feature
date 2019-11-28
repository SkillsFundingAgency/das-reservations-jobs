Feature: Reservation created
	In order to know when a reservation has been created
	I want to be told when the reservation search index is updated


Scenario: Reservation search index should be updated on reservation creation
	Given I have a reservation ready for creation
	When a create reservation event is triggered
	Then the reservation search index should be updated with the new reservation

Scenario: Employer should be notified on reservation creation
	Given I have a reservation ready for creation
	When a create reservation event is triggered
	Then the employer should be notified of the created reservation