

<header>
EZRollback Documentation
============
</header>

# Introduction

This documentation will explain how to use the framework. Before being able to use the rollback in your game engine, the framework will need to know what information need to be stored to perform a rollback and what needs to be executed when simulating frames.

# Design goals
The design goal of the framework is to enable the possibility to implement a rollback system in your Unity game. The uses are multiple :
- **Debug purpose :** Gather precise informations each frames visually.
- **Netcode :** Implement an online game using rollback.
- **Replay system** :  Record frames and play it backward and forward.

![Gif showing the use of a rollback system in a simple game](./img/RollbackIntroduction.gif)

# Integration

To add rollback into your game, you will need to adapt your scripts for the rollback to work properly.
Main things to change :

 - Determine what data need to be rollbacked and compact them into structs,
 - Make your **Monobehaviour** scripts inherit from **RollbackBehaviour** instead,
	 - Implement in the Simulate() function what needs to be evaluated each frames.
 - If needed, implement your input system with **RollbackInputManager**.

## Transitionning your scripts

### Monobehaviour with RollbackBehaviour

Basically, you will need to implement what the rollback needs to do on your scripts. For every scripts that changes important values every frames, you will need to replace :
- Implement the abstract functions from RollbackBehaviour
	- Simulate()
		- **Replaces FixedUpdate()**
		- Is called every fixed update, and when simulating back frames.
	- SetValueFromFrameNumber(int frameNumber)
	- DeleteFrames(int numFramesToDelete,RollbackManager.DeleteFrameMode deleteMode)
	- SaveFrame() 
- Put your rollbackable variable into a defined struct. Create that struct with the RollbackElement<> wrapper.
```C#
public struct SpeedValues {
    public float currentSpeedo;
    public float currentSpeedMultiplier;
    public Vector2 direction;
}

[...]

public RollbackElement<SpeedValues> rbElements = new RollbackElement<SpeedValues>();
```
Put all the Update/Fixed update needs into the Simulate(). Simulate will be called every fixed update. Puting all the update information into Simulate works too, as the rollbackManager only registers in history every fixed time step.

```C#
// Use this instead of FixedUpdate()
public override void Simulate() {
	//Your code here
}
```
**Don't use Time.deltatime in the Simulate function.** If you use it, rather use *Time.fixedtimestep* in your Simulate() function. You still can use time.deltatime with your Update() loop, but be aware that you cannot use Time.deltaTime in the Simulate() function.

### **Your input manager** with **RollbackInputManager** :
- Implement the asbtract function : GetCurrentActionsValue(), where you store your current inputs from your input manager.
	- You can implement virtual functions depending to your needs :
		- void UpdateInputStatus()
		- float **GetAxis**(AxisEnum axis, int playerId, int frameNumber = -1)
		- bool **GetInput**(int actionValue, int playerId, int frameNumber = -1)
		- bool **GetInputDown**(int actionValue, int playerId, int frameNumber = -1)
		- bool **GetInputUp**(int actionValue, int playerId, int frameNumber = -1)
		- string **GetActionName**(int actionIndex)
			- I recommend implementing this one, for debug purposes.
	- Change all input uses in your game with the one from the new manager.
	

## Example scripts

For **RollbackBehaviour** : 

- PositionRollback.cs 
- RotationRollback.cs

Implements rollback for the position and the rotation of the linked GameObject.

For **RollbackInputManager** : 
- SampleRollbackInputManager

Implementing the basic Unity input system to the needs of the rollback system. 
Can be found in Tests/Runtime/InputDelayComparer.


### Transition examples : 
Data sctruct to rollback of your class (initially used in your script)
```C#
public struct SpeedValues {
    public float currentSpeedo;
    public float currentSpeedMultiplier;
    public Vector2 direction;
}
```

Make it displayable in editor (not necessary, you can skip this step).

```C#
[Serializable]
public class RollbackElementSpeedValues : RollbackElement<SpeedValues> { }
```

Initialize your sctruct in your class.

```C#
[SerializeField] public RollbackElementSpeedValues rbElements = new RollbackElementSpeedValues();
```

Access your original value through **.value**. The wrapper behind stores an history of that value for the rollback.

```C#
float angle = Mathf.Atan2(rbElements.value.direction.y, rbElements.value.direction.x) * Mathf.Rad2Deg - 90.0f;
```

Implement the other functions by registering your RollbackElements inside of them :

```C#
public override void SetValueFromFrameNumber(int frameNumber) {
	rbElements.SetValueFromFrameNumber(frameNumber);
}

public override void DeleteFrames(int numFramesToDelete, bool fromFrames) {
	rbElements.DeleteFrames(numFramesToDelete, fromFrames);
}

public override void SaveFrame() {
	rbElements.SaveFrame();
}
```

If you're using a component from Unity to store its value, you can achieve it by doing so : 

```C#
public override void SetValueFromFrameNumber(int frameNumber) {
	_colors.SetValueFromFrameNumber(frameNumber);
	_spriteRenderer.color = _colors.value;
}

public override void SaveFrame() {
	_colors.value = _spriteRenderer.color;
	_colors.SaveFrame();
}
```

## Things to be aware of
**Don't use Time.deltatime in the Simulate function.** 
Rather use Time.fixedtimestep, else your scripts won't work when simulating frames.

**If implementing online, be aware of time synchronization.**. Have a way to know when to start recording rollback in your game, and when to stop.

## Important scripts

### Abstract classes
To implement the rollback in your game, you will mainly inherite those classes to your scripts :

**RollbackBehaviour** : Abstract class, inheriting from Monobehaviour, that implements all the required functions from the rollbackmanager callbacks.

**RollbackInputManager** : Complement abstract manager that stores player's inputs and rollback them. Is necessary to use rollback in your game for inputs(for networking for example). Have extra functions that allow input correction for players.

### Other scripts

**RollbackManager** : The main manager that deals with all the rollback mechanism. Make callbacks at the right time and manage the global status of the frames and the game.

**RollbackElement< T >** : Data structures conveniently designed to store all the information the rollback system need about your variable. Use preferably with a struct to optimize its use.

**RollbackInputBaseActions** : Base data structure used to store the input data. Optimised to use the minimum required space for network transfere.

# Glossary

**Rollback** : Going back in time in the state of the game

**Simulate frames** : Going forward in time. Works like fixed update... But we do it a certain number of time at once.

**Fixed frame** : A frame recorded with the fixed time step of Unity (default : every 0.02Sec)
