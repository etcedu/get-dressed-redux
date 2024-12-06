using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SimcoachGames.EventRecorder
{
    public class TsvParser
    {
        public class ParsedTable
        {
            public string[][] DataTable { get; }
            public int Rows => DataTable.Length;

            public ParsedTable(string[][] dataTable)
            {
                this.DataTable = dataTable;
            }

            public string[] GetRow(int row)
            {
                return DataTable[row];
            }

            public bool TryGetCell(int row, int column, out string data)
            {
                data = "";
                if (DataTable.Length <= row)
                    return false;

                if (DataTable[row].Length <= column)
                    return false;

                data = DataTable[row][column];
                return !string.IsNullOrEmpty(DataTable[row][column]);
            }

            /// <summary>
            /// Looks for "lookFor" string in each row's "searchColumn". If found returns the row it was found in.
            /// </summary>
            /// <param name="lookFor"></param>
            /// <param name="searchColumn"></param>
            /// <param name="rowNum"></param>
            /// <returns></returns>
            public bool TryFindDataInRow(string lookFor, int searchColumn, out int rowNum)
            {
                rowNum = 0;

                if (searchColumn > Rows)
                {
                    Debug.LogError("TsvParser: Search Column is out of range of table");
                    return false;
                }

                for (int i = 0; i < DataTable.Length; i++)
                {
                    string[] rowData = DataTable[i];
                    if (rowData[searchColumn] == lookFor)
                    {
                        rowNum = i;
                        return true;
                    }
                }

                return false;
            }
        }

        public static ParsedTable ParseTsvFile(string path)
        {
            string contents = File.ReadAllText(path);
            return ParseTsv(contents);
        }

        //WTP TODO: Probably should throw exceptions if the TSV is not valid
        public static ParsedTable ParseTsv(string contents)
        {
            string[] lines = contents.Split('\n');

            List<string> rowData = new();
            foreach (string line in lines)
            {
                string l = line.Replace("\r", string.Empty);
                if (!string.IsNullOrEmpty(line))
                {
                    rowData.Add(l);
                }
            }

            //Create 2D array for storing table data
            string[][] table = new string[rowData.Count][];

            //Iterate through rows
            for (int rowIndex = 0; rowIndex < rowData.Count; rowIndex++)
            {
                //Split row data into column array
                string[] columnData = rowData[rowIndex].Split('\t');

                //Init column array
                table[rowIndex] = new string[columnData.Length];

                //Iterate through column data - store in table
                for (int columnIndex = 0; columnIndex < columnData.Length; columnIndex++)
                    table[rowIndex][columnIndex] = columnData[columnIndex];
            }

            return new ParsedTable(table);
        }
    }


}