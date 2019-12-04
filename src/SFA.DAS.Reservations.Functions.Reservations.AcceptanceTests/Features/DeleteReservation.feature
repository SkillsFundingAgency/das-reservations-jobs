Feature: Reservation deleted
	In order to know when a reservation has been deleted
	I want to be told when the reservation search index is updated


Scenario: Reservation search index should be updated on reservation deletion
	Given I have a reservation ready for deletion
	When a delete reservation event is triggered by provider
	Then the reservation search index should be updated with the deleted reservation removed

Scenario: Employer should be notified on reservation deletion
	Given I have a reservation ready for deletion
	When a delete reservation event is triggered by provider
	Then the employer should be notified of the deleted reservation

Scenario: Employer should not be notified on employer created reservation deletion by employer
	Given I have a reservation ready for deletion
	When a delete reservation event for a reservation created by employer is triggered by employer
	Then the employer should not be notified of the deleted reservation

Scenario: Employer should not be notified on provider created reservation deletion by employer
	Given I have a reservation ready for deletion
	When a delete reservation event for a reservation created by provider is triggered by employer
	Then the employer should not be notified of the deleted reservation