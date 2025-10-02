# Stage 1: Build the React application
FROM node:18-alpine AS build
WORKDIR /app

# Copy package.json and package-lock.json to leverage Docker cache
COPY package*.json ./
RUN npm install

# Copy the rest of the application source code
COPY . .
RUN npm run build

# Stage 2: Serve the application using Nginx
FROM nginx:1.25-alpine AS final
# Copy the built static files from the build stage
COPY --from=build /app/dist /usr/share/nginx/html

# Nginx listens on port 80 by default
EXPOSE 80

# When the container starts, nginx will serve the static files
CMD ["nginx", "-g", "daemon off;"]
