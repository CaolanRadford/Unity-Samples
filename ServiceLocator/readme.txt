I use a service locator pattern to handle any scene wide game state that multiple gameobjects need access to.
Each service inherits from a 'Service' abstract class, and is then scooped up by the 'GM_ServiceLocator' class on initialization.
The 'GM_ServiceLocator' class then injects itself into any objects scene wide that implement it's interface by calling a method that it has defined.
Any class that wants to access a service held by the 'GM_ServiceLocator', just calls the 'GetService' method on the locator that it was handed on initialization, and then passes in the desired service. An eroor is thrown if the service is not found.
