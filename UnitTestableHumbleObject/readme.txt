I recently found out that it is possible to Unit test components within Unity using the in-built test runner window.
The difficulty in this however is dealing with monobehaviours. You can't instantiate monobehaviours directly, and so this makes it awkward to mock them for testing.
One solution to this is to use a 'Humble Object' pattern.
The idea is to strip away any behaviour from the monobehaviour class, and instead wrap any needed monobehvaiour functionality inside an interface.
Then you can write a non-monobehaviour class that references this interface and includes your originally desired behaivour.
Using nSubstitute, it is then possible to mock the mono-behaviour by mocking it's interface.
This allows us to write tests for our non-monobehaviour 'behaviour' class.
