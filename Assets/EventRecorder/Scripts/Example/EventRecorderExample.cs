using System;

/*
 *  WTP: This is an example of how to manually setup an game event integration script for your project. Don't change
 *  this one because it may be overwritten when upgrading the EventRecorder package.
 *  Steps:
 *  1. Create your EventRecorder.cs file.
 *  2. Define your class as "public static partial class EventRecorder"
 *  3. Follow the outlined structure to create your game events
 *
 *  NOTE:
 *  - The names of your classes will be used as the name of the event - so name them well!
 *  - Half of the EventRecorder class is already defined elsewhere so the "partial" keyword is important to include!
 */

namespace SimcoachGames.EventRecorder.Example
{
    public static /*partial*/ class EventRecorderExample
    {
        #region Example

        [Serializable] public class ExampleGameEvent : GameEventData
        {
            public int exampleInt;
        }

        public static void RecordExampleGameEvent(int exampleInt)
        {
            var e = new ExampleGameEvent()
            {
                exampleInt = exampleInt,
            };
            GameEventManager.RecordEvent(e);
        }

        #endregion Example
    }
}