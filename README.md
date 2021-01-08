# DynamicLoad
An example of loading (and unloading) Assembly on runtime in .NET Core 

The main application is the Runner project (console application), which dynamically loads the CustomJob project (class library). When Runner executes the CustomJob.MainJob.SayHello method, it has a dependency to Util project (another class library) and Util will also be loaded dynamically. The idea is, we can have a folder that contains a main Assembly file, along with its Assembly dependencies inside that folder, and the Runner can load and execute the main Assembly. After it has finished, the Runner will unload it. 
