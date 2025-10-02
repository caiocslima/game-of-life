# *Conway's Game of Life*

This repository contains a complete implementation of [Conway's Game of Life](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life), consisting of a RESTful API in .NET and an interactive frontend in React (TypeScript). The application is fully containerized with Docker for easy setup and deployment.

## **Prerequisites**

* Docker and Docker Compose
* Node.js and npm (to use the convenience scripts)

## **Getting Started**

1. **Clone the Repository**  
   git clone https://github.com/caiocslima/game-of-life.git

2. **Create the Environment File**  
   Copy the example .env.example file to a new file named .env.  
   `cp .env.example .env`

   *The .env file already comes with default values for the Docker environment and requires no changes to run locally.*
3. Start the Containers with Docker Compose  
   This command will build the frontend and backend images and start all containers.  
   `npm run docker:up`

4. **Access the Applications**
    * **Frontend:** Open your browser and navigate to **http://localhost:3000**
    * **API (Swagger):** To explore the API documentation, navigate to **http://localhost:8080/swagger**

## **Available Scripts (package.json)**

In the project's root folder, you can use the following npm scripts to manage the application:

* `npm run docker:up`: Starts all containers.
* `npm run docker:down`: Stops and removes all containers.
* `npm run test:backend`: Runs the .NET backend unit test suite.
* `npm run dev`: Starts the frontend development server only (useful for rapid UI development).
* `npm run lint:frontend`: Apply lint to frontend.
* `npm run build:frontend`: Build frontend project.

## **Architecture Overview**

* **Backend (.NET 7.0):** API that handles the core logic, state persistence, and real-time simulation streaming.
* **Frontend (React \+ TypeScript):** UI to create, visualize, and interact with the Game of Life simulations.
* **Database (PostgreSQL):** Database container to persist the state of the boards across sessions and application restarts.

## **Key Features**

### **Backend**

* **RESTful API:** Clear endpoints for creating boards, advancing generations, and finding final states.
* **Persistence:** The state of each board is saved in a PostgreSQL database, surviving restarts.
* **Real-Time Streaming:** A Server-Sent Events (SSE) endpoint (/stream) for continuous simulations, avoiding the overhead of HTTP polling.
* **Structured Error Handling:** Custom middleware returns errors in the ProblemDetails format.
* **Unit Tests:** A test suite with xUnit and Moq to ensure the quality and reliability of the business logic.

### **Frontend**

* **Interactive Grid:** A clickable grid to draw initial patterns, with also the possibility to create random initial states automatically.
* **Full Simulation Control:** Buttons for Play/Pause, stepping forward one generation, advancing N generations, and finding the final state. Besides that, also includes a slider for speed control.
* **Detailed State Display:** Shows the current generation number and informs the user when a final state (Stable or Oscillator) is reached.
* **Error Handling:** Displays clear error messages from the API.


