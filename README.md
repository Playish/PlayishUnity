# PlayishUnity

Playish plugin for Unity 5 used to communicate between the game engine and the playish website


# How to use the Playish plugin


## Get the plugin!

To start, drag the unity package file into your project in Unity and import the files. Create an empty GameObject in your scene and add the `PlayishManager` script to it located in `Extensions/Playish/PlayishManager`. You can now begin to use the plugin!

- Tip
`PlayishManager` is a singleton which means there should only be one instance of this in the game. The script is set to not be unloaded when you change scene in the game and handle multiple instances of the script when running. To avoid problems try to make sure there is only one instance of `PlayishManager` in your game. A good way of doing this is to have a scene that is only loaded once in your game with your `PlayishManager` in it.


## Get hold of the classes

- `PlayishManager`
This is your main connector between the game and the browser (console). Use this class to change controllers and listen for pause events. This is a singleton and a reference to this single object can be fetched using `PlayishManager.getInstance()`.

- `DeviceManager`
This is your manager for all connected devices. This is a singleton and a reference to this single object can be fetched using `DeviceManager.getInstance()`. You can find more info below.


## Request your controller

To set or change the controller that should be shown in the phone use the playish managers function `changeController(string controllerName)`. A code example could be `PlayishManager.getInstance().changeController("MySuperAwesomeController");`. This line would try to change the controller for all connected devices to your defined controller called "MySuperAwesomeController". In the future you will be able to set different controllers for a specific device but for now all connected devices shares the same controller definition.

- Keep in mind
You can also use a default controller that is defined by Playish. These are called PlayishDefaultController1 and so on with increasing numbers. You can't name your controller beginning with PlayishDefaultController* since then we will assume you are trying to get a default controller when changeing controllers for your game. (TODO: Have resource for all the default controllers with their definitions)


## Managing players

- `DeviceManager`
Manages all the connected devices that Playish handles, for now this is going to be phones. All connected devices can be found in a HashMap called `devices`. There are also useful functions to manipulate the hashmap more easily, like `getDevice(string deviceId)` that returnes the device with a specific device identifyer or null if there was no such device connected.

To keep track of when devices are added or removed the game can listen for such events by subscribing to `deviceAddedEvent` or/and `deviceRemovedEvent` (there is also an event called `deviceChangedEvent` for when one of the other two are triggered). In the event args the connected or disconnected devices deviceId can be found.

You can use the device hashmap found in device manager to use as players. If your game support multiplayer you should keep track of changes to the hashmap and manage your player in the game accordingly and keeping a deviceId reference connected to that player. This way you can get the related device to the player when you need to get the input. If the game is made as a single player game, you can just pick the first element in the hashmap.

Player related info such as player number or objects in the game should be handled by the game.

- Keep in mind
If your game relies on the events fired by the device manager make sure to check if there are already devices connected first. Setup your event callbacks in the Awake part of your MonoBehaviour object. This way you will recieve the events that might get called when your object is about to start (the plugin will set the currently connected devices in the Start part).


## Getting input

A device contains and handles its input. Use `getBoolInput(string name)`, `getIntInput(string name)` and `getFloatInput(string name)` to receive the latest sent input from the device.

- Joystick type controller component will return int values rangeing from -99 (left) to 99 (right) in the x axis. For example `getIntInput("MyJoystickX")` will return the value for a joystick x axis with the name "MyJoystick". The y axis ranges from -99 (top) to 99 (bottom). Using `getIntInput("MyJoystickY") will return the value for the joysticks y axis.

- Button type controller component will return int values rangeing from 0 to 1 where 1 is pressed and 0 is its normal state. Using `getBoolInput("MyButton")` will return true if the button called "MyButton" is pressed (getBoolInput is based on int inputs but checks if greater than 0 straight away).

- Rotation input from the controller can be recieved using `getFloatInput("rotationX")` for the x component in a rotation quaternion. With "rotationX", "rotationY", "rotationZ" and "rotationW" you can create your full quaternion to determine the rotation of the device. These float values ranges from -1 to 1. (TODO: Explain axis locations)

- Acceleration input from the controller can be recieved using `getFloatInput("accelerationX")` for the x component of the acceleration. Using "accelerationY" and "accelerationZ" you can get all acceleration values for the device. These values ranges from -10 to 10 (acceleration capped at 10 G:s). Values for acceleration is measured in G:s (9,81 m/s^2). (TODO: Explain axis locations)
