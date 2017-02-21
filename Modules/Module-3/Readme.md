
# Interaction between Users and Bots

Normally, when you create a bot, you want it to have some type of interaction with the users who are going to use it, and depending on the finality you will want to model a conversation with the user. To accomplish it you can use **Dialogs**.

![](../../images/mod3_1.jpg)
 
## Dialogs

Dialogs model a conversational process, where the exchange of messages between bot and user is the primary channel for interaction with the outside world. Each dialog is an abstraction that encapsulates its own state in a C# class that implements **IDialog**.

Let’s see an example of a Dialog to understand it a bit better. If you have completed Module 1 you can use the solution created, if not, you need to make a new Bot Framework solution. 

Add a Dialogs folder in your solution where you will store your different types of Dialogs. Inside that folder create a new class file and name it whatever you want. In my case, I have named it **MyFirstDialog**.

![](../../images/mod3_2.jpg)