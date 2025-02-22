#TaskManagementSystem

**TaskManagementSystem** is a microservices-based application developed using .NET technology. This project enables communication between microservices via MongoDB and RabbitMQ. It also provides secure communication and authorization control between APIs using JWT tokens. The application includes core functionalities like user management and task management.

## Features

- **Microservices Architecture**: The project is structured with a microservices architecture, where each functionality is presented as an independent service.
- **MongoDB Database**: User data and tasks are stored in a MongoDB database.
- **RabbitMQ**: Asynchronous communication between microservices is facilitated via RabbitMQ.
- **JWT Token**: Secure data transmission between APIs and the MVC application is achieved using JWT tokens.
- **Authorization Control**: User permissions are controlled via JWT tokens, ensuring that only authorized users can perform specific actions.
- **API Gateway**: An API Gateway is used to manage, route, and aggregate requests to different microservices. It simplifies the communication between the client and various services by providing a unified entry point.

## Technologies Used

- **.NET 8**: The core technology for building the application.
- **MongoDB**: A NoSQL database used to store user data and tasks.
- **RabbitMQ**: A message broker for asynchronous communication between microservices.
- **JWT (JSON Web Tokens)**: Used for secure data transmission and authorization between APIs and the MVC application.
- **ASP.NET Core MVC**: For building the web application.
- **ASP.NET Core Web API**: For building the APIs that interact with the front-end.
- **API Gateway**: An interface used to aggregate requests and route them to the appropriate microservices.
