I like to keep some static methods with commonly used functionality across the project in an easily accessable 'Utilities' class.
One example of this is dependency injection.
I like to have my components and scripts search for and set up any dependencies that require them.
To do this, I need to be able to find any implementations of their interface, either scene wide or within the same gameObject.

When searching the same game object for interfaces, there is difficulty in using GetComponent<> on non-monobehaviour state classes.
To get around this I wrote my own static method that searches for mono-behaviour implementations using GetComponent, as well as any non-monobehaviour classes created by the state machine, if one was found.

When searching scene wide, it is difficult to return interface implementaions from non-monobehaviours too.
To get around this I wrote another method to search for interfaces scene-wide by searching for any state machines, and then looking through any non-monobehaviours that they have created.

These solutions aren't perfect as they require me to adhere to a rigid project archtecture, so I am always on the lookout for more flexible solutions.
