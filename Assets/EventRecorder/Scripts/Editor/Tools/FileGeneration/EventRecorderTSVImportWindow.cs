using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SimcoachGames.EventRecorder
{
    public class EventRecorderTSVImportWindow : EditorWindow
    {
        const string FileName = "EventRecorder";
        const bool ImportComments = true;
        
        static string FilePath = "";
        static bool HasError;
        static string ResultMessage = "";
        static MessageType ResultMessageType;

        [MenuItem("Tools/Event Recorder/Import Event TSV")]
        static void Init()
        {
            if (string.IsNullOrEmpty(FilePath))
                FilePath = "Assets/";
            HasError = false;

            EventRecorderTSVImportWindow window = (EventRecorderTSVImportWindow) GetWindow(typeof(EventRecorderTSVImportWindow), true, "Import Event TSV", true);
            window.ShowUtility();
        }

        void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Select Source", GUILayout.MaxWidth(100)))
                FilePath = EditorUtility.OpenFilePanel("Select TSV", FilePath, "tsv");
            EditorGUILayout.LabelField(FilePath, EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();

            if (string.IsNullOrEmpty(FileName))
            {
                HasError = true;
                EditorGUILayout.HelpBox("No filename", MessageType.Error);
            }
            
            GUI.enabled = !HasError;

            if (GUILayout.Button("Try Import"))
            {
                List<EventRecorderCSFileGenerator.EventData> eventList = new List<EventRecorderCSFileGenerator.EventData>();
                if (!TryImport(FilePath, out eventList, out ResultMessage))
                {
                    ResultMessage = "Error: " + ResultMessage;
                    ResultMessageType = MessageType.Error;
                }
                else
                {

                    if (!EventRecorderCSFileGenerator.EventCreateFile(FileName, eventList, out ResultMessage))
                    {
                        ResultMessage = "Error: " + ResultMessage;
                        ResultMessageType = MessageType.Error;
                    }
                    else
                    {
                        ResultMessage = "Success!";
                        ResultMessageType = MessageType.Info;
                    }
                }
            }

            if (!string.IsNullOrEmpty(ResultMessage))
                EditorGUILayout.HelpBox(ResultMessage, ResultMessageType);
        }

        //WTP TODO: This is a mess
        static bool TryImport(string path, out List<EventRecorderCSFileGenerator.EventData> events, out string error)
        {
            events = new List<EventRecorderCSFileGenerator.EventData>();
            error = "";

            if (!File.Exists(path))
            {
                error = string.Format("File [{0}] does not exist", path);
                return false;
            }

            string csvText = File.ReadAllText(path);

            csvText = csvText.Replace(";", ","); //Replaces ; with ,

            //WTP TODO: Need to remove the stuff in-between as well
            csvText = csvText.Replace("(", ""); //Removes (
            csvText = csvText.Replace(")", ""); //Removes )

            List<string> rowSplit = csvText.Split('\n').ToList();
            rowSplit.RemoveAt(0); //Removes header line

            for (int i = 0; i < rowSplit.Count; i++)
            {
                rowSplit[i] = rowSplit[i].Replace("\r", " ");
            }

            List<List<string>> rowAndColumns = new List<List<string>>();

            foreach (string row in rowSplit)
            {
                List<string> columns = row.Split('\t').ToList();

                if (string.IsNullOrEmpty(columns[0]) || string.IsNullOrEmpty(columns[1]))
                    continue;

                //Only take the first 3 columns (Event name, params, notes)
                for (int i = 0; i < columns.Count; i++)
                    columns = columns.Take(3).ToList();

                rowAndColumns.Add(columns);
            }


            foreach (List<string> rowAndColumn in rowAndColumns)
            {
                EventRecorderCSFileGenerator.EventData newEvent = new EventRecorderCSFileGenerator.EventData();
                newEvent.name = rowAndColumn[0];

                if (ImportComments)
                    newEvent.comment = rowAndColumn[2];

                List<string> dataPoints = rowAndColumn[1].Split(',').ToList();
                foreach (string point in dataPoints)
                {
                    string[] pointSplit = point.Split(':');
                    
                    //Check to see if we marked this event as having no parameters with the "none" keyword
                    if (pointSplit[0].ToLower().Trim() != "none")
                    {
                        
                        //Check to see if we have our type info or need to add it directly in the generated script
                        if (pointSplit.Length == 2)
                        {
                            newEvent.dataPoints.Add(new EventRecorderCSFileGenerator.DataPoint()
                            {
                                name = pointSplit[0],
                                type = pointSplit[1]
                            });                
                        }
                        else
                        {
                            newEvent.dataPoints.Add(new EventRecorderCSFileGenerator.DataPoint()
                            {
                                name = pointSplit[0],
                                type = "NEEDS_TYPE"
                            });    
                        }
                    }
                }

                newEvent.Validate();
                events.Add(newEvent);
            }

            return true;
        }
    }
}