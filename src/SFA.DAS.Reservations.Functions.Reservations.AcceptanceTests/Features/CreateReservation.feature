Feature: Reservation created
	In order to know when a reservation has been created
	I want to be notified when the reservation is created and the search index is updated


Scenario: Reservation search index should be updated on reservation creation
	Given I have a reservation ready for creation
	When a create reservation event is triggered by provider
	Then the reservation search index should be updated with the new reservation

Scenario: Employer should be notified on reservation creation by provider
	Given I have a reservation ready for creation
	When a create reservation event is triggered by provider
	Then the employer should be notified of the created reservation

Scenario: Employer should not be notified on reservation creation by employer
	Given I have a reservation ready for creation
	When a create reservation event is triggered by employer
	Then the employer should not be notified of the created reservation

Scenario: Employer should not be notified on levy reservation creation
	Given I have a reservation ready for creation
	When a create reservation event is triggered for a levy employer
	Then the employer should not be notified of the created reservation