# HChatBot
This script is used to create your own chatbot, as the entity that is going to run the module. Basically, you're going to use it in the account that is going to be the bot. It's useful for creating minigames, automation bots, etc.

## Basics
Basically, you use two classes: the HChatBot, that holds all the conversation cases, and the HDialogue, that composes each dialogue.
To use it, instantiate a HChatBot class:
```CSharp
HChatBot chat = new HChatBot();
```
Instantiate a HDialogue class:
```CSharp
HDialogue diag1 = new HDialogue();
```
And then add requests (incoming) and responses (outgoing). You can have multiple responses for multiple requests (although they've got to have the same quantity):
```CSharp
diag1.AddRequest("Hello", "Hi");
diag1.AddResponse("Hello! I'm a bot.", "Hi! I'm a bot.");
```
And finally, add the _diag1_ instance to our _chat_ instance:
```CSharp
chat.AddDialogue(diag1);
```
You can also add void methods as responses. Some examples and further explanation are included in the source code.
