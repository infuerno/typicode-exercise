## Intro

Takes an average of about an hour, but no formal time limit, more interested in seeing a well thought through and complete solution.

* The task should be completed with an appropriate level of unit testing.
* Your code should trend towards being SOLID.
* Your code should compile and run in one step.
* Your solution should be written in C#, using .NET Full Framework or .NET core
* Your solution may use MSTest, NUnit or XUnit
* You may use additional frameworks/libraries/packages as needed.

## Task

Create a Web API that when called:

* Calls, combines and returns the results of:
	* http://jsonplaceholder.typicode.com/photos
	* http://jsonplaceholder.typicode.com/albums
* Allows an integrator to filter on the user id – so just returns the albums and photos relevant to a single user.