# Technical Exercise – Approach and Decisions

## Overview

This document summarizes my approach to the technical exercise, including the assumptions made, the main technical decisions, the challenges encountered, and potential future improvements.

## Understanding of the Problem

My understanding of the objective was to:

- Build an application from scratch that supports the standard CRUD operations.
- Apply software development best practices to keep the application clean, readable, and maintainable.
- Create a database to persist the application data.
- Run and test the application to verify that all core functionality works as expected.

## Assumptions

Before starting, I made the following assumptions:

- The main objective was to demonstrate the application's architecture and core functionality rather than deliver a production-ready system.
- Each user should only be able to access and manage their own data.
- A local database was sufficient for demonstrating the solution.
- Local execution instructions would be sufficient, as deployment to a shared environment was not required.
- Due to the time constraints, core functionality, code quality, and testing took priority over additional user-interface features and containerization.

## Approach

I divided the work into the following stages:

1. Analyzed the requirements and identified the core functionality.
2. Defined the application architecture, responsibilities, and data flow.
3. Designed the database model and persistence layer.
4. Implemented the essential backend and frontend features.
5. Added validation, error handling, and user data isolation.
6. Added unit and integration tests.
7. Ran the application locally and reviewed the complete user flow.

## Technical Decisions

### Backend: .NET and C#

I chose .NET and C# for the backend because they provide a mature ecosystem for building reliable and maintainable web applications. They also support dependency injection, asynchronous programming, automated testing, and a clear separation of concerns.

My previous experience with the Microsoft ecosystem allowed me to work efficiently while maintaining a structured implementation.

### Frontend: Angular

I selected Angular because it was relevant to the role and provides a structured approach to building frontend applications. Its component-based architecture, dependency injection, routing, and HTTP client made it suitable for integrating the user interface with the backend API.

### Architecture

I organized the solution using a clean, layered architecture. This keeps the application's business logic separate from infrastructure and presentation concerns, making the code easier to understand, test, and maintain.

### Containerization

I decided not to add Docker during the exercise. Given the available time, I prioritized completing and testing the core functionality. Containerization would be a valuable next step to make the development and deployment environments more consistent.

## Challenges

One of the main challenges was integrating all the application layers and ensuring that the frontend, backend, and database worked correctly together.

I also encountered issues with the Angular development proxy. To continue validating the application within the available time, I configured the frontend to use the backend URL explicitly. For a production-ready solution, I would move this value to environment-specific configuration.

## Testing

I verified the implementation through:

- Unit and integration tests written with xUnit.
- Local execution and manual testing of the main application flows.
- CRUD operation testing, including validation and error scenarios.
- Testing with two different users to confirm that each user could only access their own data.

## Trade-offs

Given the available time, I prioritized:

- A clear separation of concerns.
- Readable and maintainable code.
- Automated testing.
- Correct implementation of the core requirements.
- A functional end-to-end application.

As a result, containerization, deployment, and some user-interface enhancements were left outside the initial scope.

## Potential Improvements

With more time, I would:

- Add Docker support for the frontend, backend, and database.
- Move environment-dependent values, such as the backend URL, into environment-specific configuration.
- Improve the frontend navigation and add a complete application menu.
- Expand the automated test coverage, particularly for edge cases and frontend behavior.
- Add structured logging and application monitoring.
- Deploy the application to a test environment instead of running it only locally.
- Add a continuous integration pipeline to automate builds and tests.

## Final Notes

The solution focuses on separation of concerns and clearly defined responsibilities across the codebase. The main objective was to deliver a functional application with a clean, readable, testable, and maintainable structure while making pragmatic decisions based on the time available.