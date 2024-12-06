# Simcoach Event Recorder - 3/16/2 - Will Pyle

 ## Quick References
 - [Event Recorder Versions](https://drive.google.com/drive/folders/1ZrPACdqhjPrI42bIrii7kqo2QxXV4Gt1?usp=share_link)

 - [Event Template Sheet](https://docs.google.com/spreadsheets/d/1S741o0vPzHnrjFbsNiPcU0jLT-bCpw9YUg6Ei2W7s_w/edit?usp=sharing)
 - [Event Recorder Repo](https://github.com/etcedu/EventRecorder)

## Description
The Simcoach Event Recorder is our in-house attempt to create a simple and lightweight solution to gathering player analytics data. I have tried my best to keep developer ease of use and simplicity in mind.  This package also attempts to be [C# 5 compatible](https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-version-history) as well as NOT leverage TextMeshPro or external resources in order to increase it's backwards compatibility ([except one...](#dependencies)).  In the case we would like to switch to a more established solution ([Firebase](https://firebase.google.com/docs/unity/setup) or other...) see the [Exit Plan](#exit-plan) section.

Feedback is always appreciated 😊

## Getting Started
1. First and foremost, ensure your project is error free after adding the Event Recorder package to your project. If not, see the [Dependencies](#Dependencies) section.
2. Create an Event Integration File by either
    - (Suggested) Generate an integration file [Tools > Event Recorder > Import Event TSV] See the [Event Spreadsheets](#event-spreadsheets) section for more info.
    - Writing it yourself! See the `Assets/EventRecorder/Scripts/Example/EventRecorderExample.cs` for a basic example. However, I suggest you use the generator because this step is just busy work. There is no fun to be had here.
3. Add the `EventRecordingSystem` prefab to the first loaded scene in your project
4. Implement the static method calls provided by the Integration File (`EventRecorder.cs` is the default name if you used the generator) in your codebase. For example if you have an event called "StartGame" that takes the number of tries as a parameter your method signature should look something similar to:
    ```csharp
    static void EventRecorder.RecordStartGame(int numTries)
    ```
5. Configure the [Event Recorder Settings](#settings) through the settings window [Tools > Event Recorder > Settings].
6. Run the game with the `Show Console Messages` setting enabled. You should be able to see your events being captured in the console log.

> Note: Depending on the "use case" of your builds you may need to come back here to configure the settings based on the desired behavior of the build.

## Event Spreadsheets

Before implementing event tracking in your project it is important to coordinate what types of data will be important to track.  When creating your spreadsheet the below format should be followed. Please use the [Template Spreadsheet](https://docs.google.com/spreadsheets/d/1S741o0vPzHnrjFbsNiPcU0jLT-bCpw9YUg6Ei2W7s_w/edit?usp=sharing) to help you stick to the format. See the "Ref+erence" sheet for an example of what the generated integration file would like like given the example input. <b>Note: If the proper form is not followed the file generator may not work properly on your document.</b>

### Format
- (Column A) <b>Event Name:</b> Space tolerant. No special characters.
- (Column B) <b>Parameters:</b> A list of parameters the event will take. Parameter syntax is <b>[Name]:[Type]</b> with commas separating parameters. Space tolerant. No special characters. If you don't know what the type should be or the type is a more complex type (i.e. class containing multiple values) list the type as "other".

    <b>Example:</b> For the hypothetical event "Player Start" that includes the parameters "player name", "player location", and a complex type containing a large amount of data about the player stats "player stats" the parameter list would look like:

    ```
    player name : string, player location : vector3, player stats : other
    ```

- (Column C) <b> Comments To Include:</b> These are comments that can be optionally included in the generated .cs file. Mainly notes for developers. Space and special character tolerant.

- (Column D) <b> Other Notes:</b> Column D and following columns will not be parsed by the generator and can include any notes you would like. Mainly for cross discipline coordination.

> Note: I'm sure the generator fails on a bunch of edge cases. Please let me know if you run into an issue.

## Settings

### Runtime Options

 > NOTE: These settings are very important and can change the behavior of the Event Recorder entirely. It is important to select your settings with care. If you have any questions feel free to reach out and ask a Senior Developer!

- <b>Build Use Cases:</b> Each build you make has a specific "Use Case". This setting dictates how events are saved and sent to the backend as well as if certain runtime features are available in the build. Hopefully the names are self explanatory but...
    - <b>Development:</b> Anything related to testing! Perfect for development showcases and the like.
    - <b>Playtest:</b> If this build is going to be used in a play test! NOT for testing new features!
    - <b>App Store:</b> If this build is going to become public-facing on an app store! Absolutely not for development testing or playtests!

- <b>ID Modes:</b> The Id mode specifies how events will be associated with an ID
    - <b>Device:</b> The device will generate a GUID that every captured event will be associated with
    - <b>Session:</b> A new GUID will be generated every time the app starts. Captured events will be associated with different GUIDs for different play sessions.

- <b>Post Interval:</b> How often captured events will be sent to the backend (In seconds).

- <b>Target Endpoint:</b> Automatically set if the Use Case is "Playtest" or "AppStore". See the [Advanced](#advanced) section if you need to configure this.

> NOTE: The following Settings are Optional if the Build Use Case is "Development", ON if "Playtest", and OFF if set to "App Store".

- <b>Use Permanent Storage:</b> If captured events are saved to a "permalog" file that is never automatically deleted by the system. This is a redundancy included to ensure we never lose data during playtests as this system gets the kinks worked out.
- <b>Use Event Recorder Log:</b> If Event Recorder log messages should be saved to a txt file. This is included to help us debug issues with the Event Recorder if they occur.
- <b>Show Runtime Log:</b> Allows Event Recorder Log messages to be viewed during runtime. Long press the version display in the bottom left of the screen and press the "Open Log" button in order to activate the runtime log. Note that it only captures the last 100 messages or so. This is configurable through code.

### Editor Options
> NOTE: These settings and their related features are stripped out of builds
- <b>Show Console Messages:</b> The Event Recorder can feel like it's spamming you with messages. Flip this off if you don't want to see them. They will still be viewable in the runtime log and the Event Recorder log file.
- <b>Fake Server Responses:</b> For quick debugging.
    - <b>Post Fail:</b> If "Fake Server Responses" is true this setting will control if the fake server will respond with a failure when "receiving" post requests.

## Dependencies

### Newtonsoft's Json.NET
This system takes advantage of [Newtonsoft's Json.NET framework](https://www.newtonsoft.com/json) in order to easily serialize polymorphic types and various .net collections to json. This greatly increases developer flexibility when defining the classes we would like to serialize and should completely avoid the need for complicated classes needing to define their own serialization methods.

Depending on your version of Unity and the packages included in your project you may or may not already have Json.Net installed. If you are seeing "`error CS0246: The type or namespace name 'Newtonsoft' could not be found`" you need to install the [nuget version](https://docs.unity3d.com/Packages/com.unity.nuget.newtonsoft-json@3.0/manual/index.html) through the Package Manager.

<b>To install Json.Net:</b>
1. Open [Window > Package Manager]
2. Click the "<b>+</b>" button in the upper left
3. Select "Add Package By Name"
4. Fill in the name field as "<b>com.unity.nuget.newtonsoft-json</b>"
5. Leave the version field blank
6. Click "Add"

### Backup Json Serializer
Unity has made quite the mess when it comes to integrating the Json.NET assembly into older versions of Unity (pre-2020 or so). Some versions included by default a version of Json.NET that Unity had edited and then labeled as "For internal use only." <i>Of course</i> the assembly names were the same as the normal version of Json.NET so if you tried to include your own version you would get an error.

For this reason I have also included a backup serializer [(simple json)](https://github.com/facebook-csharp-sdk/simple-json) that may work for our "legacy" projects we want to add Event Tracking to but don't feel like it is worth our time to upgrade to the latest Unity version or add the Json.Net assemblies ourself.

In order to activate the backup serializer open `EventRecorderJSONHelper.cs` and uncomment the line "`#define USE_BACKUP_SERIALIZER`"

> NOTE: Simple Json is capable of more than Unity's built in serializer but not as much as Json.Net. I am not sure exactly where the line is. It is very likely that in a future version I will remove simple Json and instead opt to use the built in unity json serializer.

### Licenses
Both Json.Net and Simple Json are under the MIT license


## Known Issues
 - The EventBuffer creates a lot of garbage that needs to be collected.
 - The system is reliant on Unity's coroutines for it's non-blocking read/write and post functions. If we want to adapt this system into it own managed DLL we probably want to change these features to work using .net's async functions.
 - Some older versions of Unity will throw a fit about the Json.net dependency
 - The backup serializer may serialize data differently than Json.net.
 - All Backlog files are saved in a folder titled "User_[GUID]" regardless of the Id Mode that is being used. This makes it impossible to tell what the id mode was used.

## Advanced
### General Behavior
The entry point for the Event Recording system is `GameEventManager.cs` script which is attached to the `EventRecordingSystem` prefab that all projects should include in their first loaded scene.  This script handles kicking off the "Event loop" which is a coroutine called `EventProcessCoroutine`.  When an event occurs the event info is stored in a buffer that the EventProcessCoroutine will intermittently write to disk. These files are referred to as the "backlog" files. Intermittently, based on the set `Post Interval` the `EventProcessCoroutine` will read the updated backlog files and attempt to post them to the backend.

Other important files are:
- `EventPoster.cs`: Contains methods related to posting
- `EventRecorderId.cs`: Contains methods relating to generating / changing Ids
- `EventRecorderStorage.cs`: Contains methods that read/write to the backlog and permalog files

### Structure and Storage of Game Events

All game events are an extension of this class:

```CSharp
[Serializable] public abstract class GameEventData {}
```

A hypothetical game event called "GameOver" that stores a final player score would end up looking like this:

```CSharp
[Serializable] public class GameOverGameEvent  : GameEventData
{
    int score;
}
```

When stored in the backlog the GameEventData is wrapped in a "GameEvent" class that stores more contextual information about the event:

```CSharp
[Serializable] public class GameEvent
{
    public string gameId;
    public string gameVersion;
    public string eventType;
    public string timestamp;
    public bool isDebug;
    public GameEventData eventData;
};
```

These are stored (admittedly, sub-optimally) in backlog and permalog files in an "unwrapped" json array that looks something like this:

```JSON
{
  "gameId": "EventRecorder",
  "gameVersion": "1.3.0",
  "eventType": "ExampleGameEvent",
  "timestamp": "2023-03-13T16:26:58",
  "isDebug": true,
  "eventData": {
    "exampleInt": 0
  }
},
{
  "gameId": "EventRecorder",
  "gameVersion": "1.3.0",
  "eventType": "ExampleGameEvent",
  "timestamp": "2023-03-13T16:27:09",
  "isDebug": true,
  "eventData": {
    "exampleInt": 1
  }
},
```
> Note: Captured events in the editor and Development "Use Case" builds will always have "isDebug" TRUE.

This works well enough because it is easy to append new events to the end of the file. When we are ready to post the file is read and the file contents are wrapped with square brackets and placed into a structure containing the user Id the events were saved under. The above data being sent to the server looks something like this:

```JSON
{
  "user_id": "bd92a6d0-c358-484f-9b0f-1f4232a575c4",
  "payload_items": [{
    "gameId": "EventRecorder",
    "gameVersion": "1.3.0",
    "eventType": "ExampleGameEvent",
    "timestamp": "2023-03-13T16:26:58",
    "isDebug": true,
    "eventData": {
      "exampleInt": 0
  }
},
{
  "gameId": "EventRecorder",
  "gameVersion": "1.3.0",
  "eventType": "ExampleGameEvent",
  "timestamp": "2023-03-13T16:27:09",
  "isDebug": true,
  "eventData": {
    "exampleInt": 1
  }
}]}
```

All of this data is stored in the Persistent data path of the application. You can easily open this folder by selecting [File > Open Persistent Data Path] in the Unity Editor.

The Folder Structure is as follows:

```
- \Persistent Data Folder\
    - Users.txt
    - EventRecorderLogs\
        - Log[Date].txt
        - Log[Date].txt
        - ...
    - User_[GUID]\
        - Backlog_[GUID].json
        - Permalog_[GUID].json (Depending on options)
    - User_[GUID]\
        - Backlog_[GUID].json
        - Permalog_[GUID].json (Depending on options)
    - ...
```

> NOTE: Users.txt is where the device Id GUID is saved. It is a txt file that only includes that guid. It later may become a more complex file if we end up having multiple local "user accounts" or something similar.

> NOTE: Despite the .json extension the Backlog / Permalog files are not valid json until wrapped with square brackets during the post step

### Changing Endpoints
> NOTE: I slapped this together to aid in my debugging of the Event Recorder system. It's not really meant to do anything more than that currently.

Endpoints are represented by instance of the ScriptableObject `EventRecorderEndpointSO`. Two instances are included in the project as defaults:
 - Simcoach_AWS
 - LocalHost

 The Settings window requires a reference to an Endpoint scriptable object to function. If the Build Use Case is set to anything other than "Development" it should force that reference to the the Simcoach_Aws instance. The LocalHost instance I was using to test the system and pointed at a node.js server running locally on my system.

 If there is ever a need to have certain projects point to different (or multiple) endpoints I would extend this functionality.

### Changing Serialization from Json
If we want to change our serialization format to binary or something else the `GameEventManager.RecordEvent()` method is the place to start. This is where the game event is currently turned into json before being passed to the EventRecorderLog and EventRecorderStorage systems.

```CSharp
...
    string gameEventJson = EventRecorderJSONHelper.SerializeObject(gameEvent, JsonFormatting.PRETTY);
        
    EventRecorderLog.Log(string.Format("[{0}] Event Occurred", gameEvent.eventType), string.Format("[{0}] Event Occurred: {1}" gameEvent.eventType, gameEventJson));

    EventRecorderStorage.StoreEvent(EventRecorderId.GetCurrentId(), gameEventJson);
...
```

Some changes will then need to be made to the `EventRecorderStorage.cs` file. This <i>may</i> include changing the `EventBuffers` type and the `WriteBuffersToDisk()` method depending on the new format.

The final thing to consider is how the data will be converted into json to be posted to the server. The `EventRecorderStorage.TryGetBacklogString()` method is where to look. Keep in mind that we are leveraging a polymorphic data structure (`GameEventData`) to make saving our data easier. Make sure you are prepared to deserialize this type of data. The "eventType" field of the `GameEvent` class is actually the name of the inheriting class and therefore could be used to help deserialize your data as the correct type.

### Exit Plan
If we ever want to move away from this system entirely we should be able to change the `GameEventManager.RecorderEvent()` method to do whatever we would like with the data. If for whatever reason that doesn't work, all of the projects should be calling to record their various events by using the "Integration File" static methods. So behavior could be overridden there as well.

### Creating New Event Recorder Packages

I have built a custom "Packaging Wizard" to help build the event recorder packages. This is because manually selecting the files you want to include every time a new version is made leads to human error. Instead, this wizard will build the package out of the files inside of the targeted "Package Source" folder (by default `Assets/EventRecorder`). It will also automatically update the changelog and EventRecorderVersion.txt file with your included info as well as save this new info as a part of the "Last Package Info" section for next time. This has made the iteration process MUCH easier.

If you need to update the Event Recorder package please follow these steps:
 1. If you don't have the repo you can find it [HERE](https://github.com/etcedu/EventRecorder)
 2. Make your changes
 3. Open the Packaging Wizard in the Unity Editor [Tools > Packaging Wizard]
 4. Fill out the Author Name, Version, and Changelog sections
 5. Press "Export Package" to build your Unity package
 6. Upload to the [Event Recorder Versions](https://drive.google.com/drive/folders/1ZrPACdqhjPrI42bIrii7kqo2QxXV4Gt1?usp=share_link) folder on Google drive

 Keep in mind: only the files inside of the "Package Source" folder will be built into the Unity package. This is a feature! You can write as many tests as you need for the system in this Unity project without worrying about accidentally including them.

> NOTE: The Export Package button will not be available if their are errors present. You can bypass the saftey checks with the boolean in the top left of the packaging wizard but it will not update your changelog, version number, or save this information in the "Last Package Info" section. I DO NOT advise you use it.

> NOTE: Once the main iteration phase of this package is over it may benefit us to host a package that can be downloaded through Unity's package manager on github. I hadn't done this originally because of the extra maintenance it takes being too annoying to keep up with while constantly iterating. When this system is more stable maybe it will become a better idea.
## Authors
William Pyle - Please contact me if you have any questions ⭐