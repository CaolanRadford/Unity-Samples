These are some examples of components I have written to attach to Unity game objects. They add functionality in modules. 
Most components are written in an observer/subscriber pattern, whereby a component will define an interface that any other scripts/components need to implement in order to access it's functionality. 
Each component searches the gameObject it is attached to for any implementations of it's interface to automatically set up any dependencies, such as subscribing to events, or updating a certain property, etc.
