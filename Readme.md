# User Stories

- As a user, I want to be able to see a list of all the available movies and filter by title, year of release or both.
- As a user, I want to be able to see the details of a movie.
- As a user, I want to be able to see the list of all the  movies and search for a movie by its name or director
- As a admin user, I want to be able to add a new movie to the list of movies.
- As a admin user, I want to be able to update the details of a movie.
- As a admin user, I want to be able to delete a movie from the list of movies.

# Work log:
1. I started by creating the user stories, new repository on GitHub and and the project structure following clean architecture principles 			
2. I created the models, validators and services.  I will structure the layers by feature folders in the domain and application layer 
3. I started the implementation of the Services and repositories
4. I Created the implementation of the repositories I decided to use SQL Server, but since we can't use dapper or entity framework and went and use ADO.NET directly.
5. I was thinking on changing the db to mongoDB, but it is to late already spent a lot of time implementing the repositories (with ado.net :( ). Maybe when I finish everything I will switch
6. I will now continue with the API endpoints. I will leave the tests at the end because I have limited time. I know normally the test  should go first but unfortunatly I had an emergency situation that took a lot of the allocated time I had for this challenge.		
I think is better to push on with delivering a working software and then go back and add the tests.
1. I also decided to use exception handling middleware to handle bussiness rules. I would have use the result pattern but I think it would be overkill for this small project.
1. Created a database initializer to seed to database and create database. I decided to use fluent migrator for this. I just easier and super light weight.
1. I will start implementing the authentication and users. I Decided to use BCrypt to hash the password cause the follow the latest hashing standards and is very secure.
1. Add JWT Authentication
1. Created the docker file and docker-compose file to run the application in a container
1. Add some unit tests

# Running the Application with Docker Compose

1. Install Docker and Docker Compose on your machine.
2. Navigate to the root directory of the project where the `docker-compose.yml` file is located.
3. Run the command `docker-compose up --build`. This command will build the Docker images and start the containers.
4. The application will be available at `http://localhost:8080`.

# Running the Application with LocalDB on Windows

1. Install .NET 5.0 SDK on your machine.
2. Install SQL Server Express LocalDB on your machine.
3. Navigate to the root directory of the project where the `.csproj` file is located.
4. Update the `ConnectionStrings__DefaultConnection` 
and `ConnectionStrings__MasterConnection` in the `appsettings.json` 
file to use your LocalDB instance. 
The connection strings should look something like this:
```Json
"ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=MoviesDb;Trusted_Connection=True;MultipleActiveResultSets=true",
    "MasterConnection": "Server=(localdb)\\mssqllocaldb;Database=master;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

#Important: Do not change the "MoviesDb" database name. The application uses this name to seed the database.


# Admin User

The application includes an admin user that is created when the database is seeded. The admin user has the following credentials:

- Email: admin@supersecret.com
- Password: superadmin

This user has administrative privileges and can add, update, and delete movies.

# Authenticating with the User/Login Endpoint

To authenticate and get a token, you can use the `user/login` endpoint.
Send a POST request
```Json
{
  "email": "admin@supersecret.com",
  "password": "superadmin"
}
```