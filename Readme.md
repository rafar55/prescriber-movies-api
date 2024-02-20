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
