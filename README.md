# LINQ with Cassandra

This application demonstrates how to use LINQ with Cassandra in C#

Contributors: [Dave Bechberger](https://github.com/bechbd) adapted from [here](https://docs.datastax.com/en/developer/csharp-driver/3.12/features/components/linq/)

## Objectives

* To demonstrate how to use LINQ to insert data
* To demonstrate how to use LINQ to retrieve data
* To demonstrate how to use LINQ to update data
* To demonstrate how to use LINQ to delete data
  
## Project Layout

* [Program.cs](Program.cs) - The main application file 
* [User.cs](User.cs) - The POCO object that maps to the Cassandra table

## How this Sample Works
This sample works by first making a connection to a Cassandra instance, this is defaulted to `localhost`.  Once the `Cluster` object has been built the mapping between the `User` object and the `users` table is created using this code:

```
MappingConfiguration.Global.Define(
    new Map<User>()
    .TableName("users")
    .PartitionKey(u => u.UserId)
    .Column(u => u.UserId, cm => cm.WithName("id")));
```
One thing to notice with this code is that we are mapping the `UserId` property in the object to the `id` column in the table.

Once we have made this can connected our session this sample contains four functions:

* InsertOperations - This contains the insert operations you can perform in LINQ
* QueryOperations - This contains the query operations you can perform in LINQ
* UpdateOperations - This contains the update operations you can perform in LINQ
* DeleteOperations - This contains the delete operations you can perform in LINQ

This sample shows the most common patterns used with the LINQ Method syntax.  The same functions are also available using the Query syntax.  This is not an exhaustive example of the features and configuration options available within the driver.  For a complete listing of the features and configuration options please check the documentation [here](https://docs.datastax.com/en/developer/csharp-driver/3.12/features/components/linq/).

## Setup and Running

### Prerequisites
* .NET Core 2.1
* A Cassandra cluster

**Note** This application defaults to connecting to a cluster on localhost. These parameters can be changed on line 28 of Program.cs.

### Running
To run this application use the following command:

`dotnet run`


This will produce output similar to the following:

```
Retrieved 5 users
Retrieved 1 users
Retrieved UserId: 32e40afe-27e2-4509-9df9-a89468993d68, Name: User 0, Age: 0
Retrieved UserId: 32e40afe-27e2-4509-9df9-a89468993d68, Name: User 0, Age: 0
```

