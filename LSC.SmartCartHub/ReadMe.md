# E-Commerce Microservices Application Series
This project covers part of the E-Commerce Microservices Application. We will build various Azure functions that supports the e-Commerce application.

# Function details
Following functions are created.

## UpdateUserProfile
This function will take User's Profile as input and then Create/Update the user in UserProfile table. This function is an Http Trigger Http Post function.
This function uses EF Core

## GetUserRoles
This function is a HttpTrigger function (Http Get) this take user's adObjId as input and fetches user's role from the database. This function uses 
EF Core

## Video Link
- How to use .Net Core, EF Core with Azure Http Trigger  functions https://youtu.be/S7kP-vqwRww

## SignUpValidation
This function will be invoked by Azure AD B2C by passing user claims. In turn this function will talk to above two functions and create a user profile based on 
incoming user details. It also inserts user role based on some logic in function. 
finally, it fetch the User roles and pass the userid and userrole in claims.
