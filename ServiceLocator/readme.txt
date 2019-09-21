I use a service locator pattern to handle any scene wide game state that multiple gameobjects need access to.
Each service inherits from a 'Service' abstract class, and is then scooped up by the 'GM_ServiceLocator' class.
The 'GM_ServiceLocator' class then injects itself into any objects scene wide that implement it's interface.
Any class that wants to access a service held by the 'GM_ServiceLocator' then just calls the 'GetService' method and passes in the desired service.
