This is the state machine design for my main character 'Ratbag'.
The 'brain' script features two dictionaries of potential states, one for animation states, and one for gameplay states. It also features methods to swap these states in and out.
These State classes are non-monobehaviour, and are based on a series of abstract classes which define common functionality across the different types of states.
State classes are swapped around by the states themselves, so each state defines it's own exit conditions and transitions.
