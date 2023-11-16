# SmartApps.Jobs
This project was developed to meet a specific client's need, and the purpose of this material is to expose the context and rationale behind the decisions made in the development of this project.

### Disclaimer
The source code presented here is only part of the project. It should not be considered as a production ready project.

### Context
Part of the asynchronous communication between the ERP and external tools is done via a message queue in databases managed by the ERP itself. When an external system needs to generate a task to be executed in the ERP, it makes a POST to an API of the ERP, which, in turn, records the message in a database to be later executed by the workers of the ERP.

This communication structure brings some characteristics that have negatively impacted the processes:

* Messages and processing logs are recorded in the same ERP database, causing the message queue execution environment to directly impact the ERP environment (e.g., consumption of licenses, database locks, etc.);
* The execution context of messages is restricted to the ERP. It is not possible to delegate part of the process execution to other systems;
* The client needs direct access to the ERP to publish messages;
* There is no effective treatment for duplicated messages, as well as treatments for retries in case of failures;

### Proposal
* Remove from the ERP the endpoints and routines that manage the messages;
* Replace the database queues with RabbitMQ;
* Create a .NET CORE API to receive requests and publish them to the exchange for processing;
* Create a worker in the ERP to consume the job queue;
* Structure the execution logs so that they are published in Elasticsearch, with a job.id field;
* Use Elasticsearch logs as the source of job execution logs;

### Expected SmartApps.Jobs features:
* Expose an endpoint to create jobs;
* Expose an endpoint to check the status of a job, including the execution logs generated in Elasticsearch;
* The job should be created only when there is no job with the same handler and payload pending or being processed;
* Allow per handler timeout configuration;

### Source Code Structure
The project's source code has been structured according to Clean Architecture standards. Despite being a small project, I chose to place each layer in a separate project for better clarity and control of dependencies in each layer;

* src/Infrastructure/: Common libraries used by various projects;
* src/Services/Jobs/: Source code of the project discussed in this document;

### Tests
The project and its libraries have unit and integration tests. For integration tests, it is necessary for the environment to have Docker installed;

### Code Coverage
In the "coverage" folder, you will find a docker-compose.yml to set up a Sonarqube environment. The project can be submitted to Sonarqube using the "docker-compose up coverage" command.

![alt text](coverage/current-code-coverage.png?raw=true)




