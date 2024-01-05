# E-Commerce Microservices Application Series
This project covers part of the E-Commerce Microservices Application. We will build various Azure functions that supports the e-Commerce application.

# Function details
The following functions are created.

## UpdateUserProfile
This function will take User's Profile as input and then Create/Update the user in UserProfile table. This function is an Http Trigger Http Post function.
This function uses EF Core

## GetUserRoles
This function is a HttpTrigger function (Http Get) this take user's adObjId as input and fetches user's role from the database. This function uses 
EF Core

## Video Link
- How to use .Net Core, EF Core with Azure Http Trigger  functions https://youtu.be/S7kP-vqwRww

## SignUpValidation
This function is invoked by Azure AD B2C via API Connector. This call is invoked before the token is generated and issued to the caller. This function receives the request, talks to other functions to create a user and its role in the database and then returns the UserId and User Roles in the claims.
