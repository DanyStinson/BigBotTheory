# Module 1 - Bot Framework Basics

Welcome to Module 0, in this section you will create your first bot application and learn the basic structure of the project.

:sos: Before we start, I strongly suggest you make sure you downloaded all the necesary software and files needed specified in [Module 0](https://github.com/DanyStinson/BigBotTheory/tree/master/Modules/Module-0)

The first step, as you will imagine is to **Open Visual Studio**.

After that, create a new project, select the Bot Application template. Assign your project a name and give life to your first bot! 

**Note**: Check the "Add to Source Control" option, it will help with you with version control and you will have a repository to work with.

![](../../images/mod1_1_1.png)
## Project structure

Lets have a look a the projects structure to understand how our bot works.

![](../../images/mod1_1_2.png)

We have: 
- A series of **properties** and **references** as we have in all our Visual Studio projects. 
- An **App_Start** folder that contains **WebApiConfig.cs** which is in charge of the routes of our Bot and Json options.
- A **Controllers** folder, it contains the controllers which will process the users actions.
- A default webpage **default.htm** which will appear when we run our project.
- A series of configuration files of the packages and WebApi.

For the moment that is all our bot needs to work. 

### **MessageController.cs**
Right now MessageController.cs is our principal class in our new project. Here is where the magic begins, specifically with the following method:

```sh
 public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                // calculate something for us to return
                int length = (activity.Text ?? string.Empty).Length;

                // return our reply to the user
                Activity reply = activity.CreateReply($"You sent {activity.Text} which was {length} characters");
                await connector.Conversations.ReplyToActivityAsync(reply);
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }
``` 


The method receives an **Activity** object (also called **activity**) that is used for communication between the user and the bot. This activity can be of different types, among them is the message type, which contains information sent between the two ends of the conversation.

**Reminder**: This template consists of a bot that returns the number of characters of the message sent by the user.

If the activity that the bot receives is of type message, it will get its length, it will generate a response with the **CreateReply ()** method and it will be sent back to the user. Then you will wait for the user to send you a message again.

If the activity is not message type, it will be redirected to another function of the controller that is in charge of actions that we can predefine for the different types of activities sent to the bot.


### Run the bot application

So... You have already seen above how a bot is structured and how it responds to the messages that it receives from the user. So let's get our bot up and running!

**Step 1**: Run the Bot Framework project. A browser window will open that will display the default.htm.

![](../../images/mod1_1_4.png)

**Step 2**: Open the Bot Framework Emulator and enter in the address bar:
```sh
Http: // localhost: {Port on which you are running} / api / messages
```

At the moment you won´t have to enter anything in the Microsoft App ID or Microsoft App Password fields because we have not registered our bot in the Microsoft Bot Framework.

![](../../images/mod1_5.png)
 
Once connected, the message "POST 200 [conversationUpdate]" will appear on the right screen of the application. This means that the bot is waiting for you to send a message. 

Go ahead, say hello!

![](../../images/mod1_1_6.png)
 
Congratulations, you have already created your first bot!

As you can see is a fairly simple process, but will be more complex as we add new features to make it smarter.

Go ahead to [Module 2](https://github.com/DanyStinson/BigBotTheory/tree/master/Modules/Module-2) to learn how to publish your bot.