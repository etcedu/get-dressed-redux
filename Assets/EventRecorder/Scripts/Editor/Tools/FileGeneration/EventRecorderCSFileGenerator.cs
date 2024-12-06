using System.Collections.Generic;
using UnityEditor;
using System.IO;

//WTP TODO: Fix bug where parameter-less events are skipped during creation
namespace SimcoachGames.EventRecorder
{

    //WTP NOTE: The weird spacing in the strings is necessary for formatting
    public class EventRecorderCSFileGenerator
    {
        public class DataPoint
        {
            public string name;
            public string type;

            public void Validate()
            {
                name = name.Replace(" ", ""); //Removes spaces
                name = name.Replace("?", ""); //Removes ?
                name = name.Replace("!", ""); //Removes !

                type = type.Replace(" ", ""); //Removes spaces
                type = type.Replace("?", ""); //Removes ?
                type = type.Replace("!", ""); //Removes !

                //Makes first character lowercase
                name = name.Substring(0, 1).ToLower() + name.Substring(1);
                type = type.Replace(" ", "");

                //WTP TODO: Probably a better way to validate using the Type class
                switch (type)
                {
                    case "Int":
                        type = "int";
                        break;

                    case "Float":
                        type = "float";
                        break;

                    case "Bool":
                        type = "bool";
                        break;

                    case "String":
                        type = "string";
                        break;

                }
            }
        }

        public class EventData
        {
            public string name;
            public List<DataPoint> dataPoints = new List<DataPoint>();
            public string comment;

            public void Validate()
            {
                var wordSplit = name.Split(' ');
                for (var i = 0; i < wordSplit.Length; i++)
                {
                    var word = wordSplit[i];
                    word = word.ToLower();
                    word = word.Substring(0, 1).ToUpper() + word.Substring(1);
                }

                name = "";
                foreach (var word in wordSplit)
                    name += word;

                //Remove newlines from comment
                comment = comment.Replace("\n", "");
                comment = comment.Replace("\r", "");

                foreach (var dataPoint in dataPoints)
                    dataPoint.Validate();
            }
        }

        public static bool EventCreateFile(string filename, List<EventData> events, out string errorMessage)
        {
            //WTP TODO: Add error message. Allow fail.

            errorMessage = "";
            var fileContents = CreateFileString(events);
            WriteStringToFile(filename, fileContents);

            return true;
        }

        static string GetDeclarations()
        {
            return
                "using UnityEngine;\n" +
                "using System.Collections;\n" +
                "using System;\n" +
                "\n";
        }

        static string BeginClass(string className)
        {
            return string.Format("public static partial class {0}\n", className) +
                   "{\n";
        }

        static string EndClass()
        {
            return "}";
        }

        static string CreateEvent(EventData eventData)
        {
            return CreateEventClass(eventData) + CreateRecordMethod(eventData);
        }

        static string CreateEventClass(EventData eventData)
        {

            var s = "";

            //Include comment if one exsists
            if (!string.IsNullOrEmpty(eventData.comment))
                s += string.Format("    //{0}\n", eventData.comment);

            s +=
                string.Format("    [Serializable] public class {0}Event : GameEventData\n", eventData.name) +
                "    {\n" +
                CreateClassMembers(eventData.dataPoints) +
                "    }\n\n";


            return s;
        }

        static string CreateClassMembers(List<DataPoint> members)
        {
            var s = "";
            foreach (var member in members)
            {
                s += string.Format("        public {0} {1};\n", member.type, member.name);
            }

            return s;
        }

        static string CreateRecordMethod(EventData eventData)
        {
            return
                string.Format("    public static void Record{0}Event({1})\n", eventData.name,
                    CreateParams(eventData.dataPoints)) +
                "    {\n" +
                string.Format("        var e = new {0}Event()\n", eventData.name) +
                "        {\n" +
                CreateRecordMethodClassConstructor(eventData.dataPoints) +
                "        };\n" +
                "        GameEventManager.RecordEvent(e);\n" +
                "    }\n\n";
        }

        static string CreateRecordMethodClassConstructor(List<DataPoint> paramList)
        {
            var s = "";
            foreach (var typePair in paramList)
            {
                s += string.Format("            {0} = {1},\n", typePair.name, typePair.name);
            }

            //WTP TODO: Remove sections
            // s += "            includeInSections = includeInSections,\n";

            return s;
        }

        static string CreateParams(List<DataPoint> paramList)
        {
            var s = "";
            for (var i = 0; i < paramList.Count; i++)
            {
                var pair = paramList[i];
                s += string.Format("{0} {1}", pair.type, pair.name);

                if (i < paramList.Count - 1)
                    s += ", ";
            }

            //WTP TODO: Remove sections
            // s += ", bool includeInSections = false"; 

            return s;
        }

        static string CreateFileString(List<EventData> events)
        {
            var s = GetDeclarations() + BeginClass("EventRecorder");

            foreach (var @event in events)
                s += CreateEvent(@event);

            s += EndClass();

            return s;
        }

        static void WriteStringToFile(string filename, string contents)
        {
            var path = string.Format("Assets/{0}.cs", filename);
            if (File.Exists(filename) == false)
            {
                using (var outFile = new StreamWriter(path))
                {
                    outFile.Write(contents);
                }
            }

            AssetDatabase.Refresh();
        }
    }
}