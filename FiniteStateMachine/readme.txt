This is the state machine design for my main character 'Ratbag'.
The 'brain' script features two dictionaries, one for animation states, and one for gameplay states. It also features methods to swap states these states in and out.
State classes are non-monobehaviour, and are based on a series of abstract classes which define common functionality across the different types of states.
State classes are swapped around by the states themselves, so that each state defines it's own exit conditions.
