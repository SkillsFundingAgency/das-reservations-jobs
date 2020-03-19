# das-reservations-jobs

## Build Status

![Build Status](https://sfa-gov-uk.visualstudio.com/Digital%20Apprenticeship%20Service/_apis/build/status/Manage%20Funding/das-reservations-jobs?branchName=master)

## Requirements

DotNet Core 2.2 and any supported IDE for DEV running. Azure function SDK v2 is also required.

## About

The reservations jobs repository provides Azure Functions to help maintain data within the reservations database. It directly relies on the database implementation provided in [das-reservations-api](https://github.com/SkillsFundingAgency/das-reservations-api) for the datacontext. 

## Local running

You are able to run in **DEV** mode the Jobs functions using an in memory database by doing the following:

- Clone repository
- Set EnvironmentName to **DEV** in local.settings.json

All chances will now happen in the in memory db context. Further Azure function capabilities are described below.

You are also able to run in **LOCAL** mode, for this you need the following dependencies:

- Docker
- SQL Server, can be hosted on docker - the database that is part of the [das-reservations-api](https://github.com/SkillsFundingAgency/das-reservations-api) should be published
- Elastic Search running on docker
- Azure Storage - an entry should be created with a PartitionKey of **LOCAL** and a **RowKey** of `SFA.DAS.Reservations.Jobs_1.0` and a **Data** property

Your configuration file for Data should look like the following:

```
{
  "ReservationsJobs": {
    "ConnectionString": "Data Source=.;Initial Catalog=SFA.DAS.Reservations;User Id=[YOUR USERNAME];Password=[YOUR PASSWORD];Pooling=False;Connect Timeout=30",
    "ApprenticeshipBaseUrl": "[URL for apprenticeship info service]",
    "AzureWebJobsStorage": "DefaultEndpointsProtocol=https;AccountName=[ACCOUNT_NAME];AccountKey=[ACCOUNT_KEY];EndpointSuffix=core.windows.net;",
    "ElasticSearchPassword":"[ELASTIC_PWD]",
    "ElasticSearchServerUrl":"[ELASTIC_URL]",
    "ElasticSearchUsername":"[ELASTIC_USERNAME]"
  },
  "NotificationsApi": {
    "ApiBaseUrl": "",
    "ClientToken": ""
  }
}
```
- it is also necessary to have the `SFA.DAS.Encoding` and `SFA.DAS.EmployerAccountAPI` encoding for running the reservations function

The app.settings file should then be as its stored in Git.

## Functions

### SFA.DAS.Reservations.Functions.LegalEntities
This function is responsible for consuming events from the Employer Apprenticeship Service. It handles the following nServiceBus events:

- CreatedAccountEvent
- ChangedAccountNameEvent
- LevyAddedToAccount
- AddedLegalEntityEvent
- RemovedLegalEntityEvent
- SignedAgreementEvent

Account information is stored in the `Account` table - account information is only ever created or updated
The following events are stored against the `AccountLegalEntity` table. For an account to use the reservation journey it must be marked as non-levy and have an employer agreement signed. If a `LevyAddedToAccount` event is received then the employer account will no longer be required to do the reservation journey or have a signed legal agreement.

### SFA.DAS.Reservations.Functions.ProviderPermission
This function is responsible for consuming events from ProviderPermissions. It handles the following nServiceBus events:

- UpdatedPermissionsEvent

Provider permission information is stored for use of building the elastic search index used for providers. When an event is received we add or remove the employers reservations for that provider to be able to search against.

### SFA.DAS.Reservations.Functions.RefreshCourse
This is a timed job, runs at midnight and refreshes all Standards into the `Course` table pulling data from the Apprenticeship Info Service API. It can also be manually invoked via a HTTP POST request.

### SFA.DAS.Reservations.Functions.ReservationIndex
This is a timed job, runs at midnight and refreshes all indexes for all providers. It can be manually invoked via a HTTP POST which will rebuild all indexes. 

### SFA.DAS.Reservations.Functions.Reservations
These functions deal with changing the state of reservation information and consume the following nServiceBus events:

- DraftApprenticeshipCreatedEvent
- DraftApprenticeshipDeletedEvent
- ReservationCreatedEvent
- ReservationDeletedEvent

The first two DraftApprenticeship Events come from the Approvals/Commitments solultion. The CreatedEvent marks a reservation as used so a further apprenticeship cannot be assigned to it. The DeletedEvent frees that reservation up to be used against another apprenticeship. The two Reservation events are triggers for the elastic index to be updated with those reservations for the providers that have permissions. 

Also off the back of any reservation created or deleted by a provider, if that employer has their notification settings set to receive emails, then they will get an email informing them that someone has deleted or created a reservation on their behalf.
