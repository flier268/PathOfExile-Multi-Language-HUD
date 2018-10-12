using LibDat;
using System;
using System.Collections.Generic;

namespace Analyzer
{
    public class AnalyzeDatcs
    {
        public static string ConvertDatToCSV(string file)
        {
            try
            {
                var dat = new DatContainer(file);
                var csvData = dat.GetCsvFormat();
                if(csvData==null)
                    throw new Exception("ConvertDatToCSV Error,is DatDefinitions.xml exist?");
                return csvData;                
            }
            catch (Exception e)
            {                
                throw;
            }
        }
        /// <summary>
        /// Convert CSV to twice dictionary
        /// </summary>
        /// <param name="csv">[0]:Key,Value  [1]:Value,Key</param>
        /// <returns></returns>
        public static Dictionary<string,string>[] GetDictionary(string csv)
        {
            var lines = csv.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length <= 0)
                return null;
            int column = lines[1].Split(',').Length;
            int row = lines.Length - 1;
            Dictionary<string, string> temp1 = new Dictionary<string, string>(row - 1);
            Dictionary<string, string> temp2 = new Dictionary<string, string>(row - 1);
            for (int i = 0; i < lines.Length; i++)
            {
                if (i == 0)
                    continue;
                var line = lines[i].Split(',');
                if(!temp1.ContainsKey(line[2]))
                temp1.Add(line[2], line[6]);
                if (!temp2.ContainsKey(line[6]))
                    temp2.Add(line[6], line[2]);
            }

            return new Dictionary<string, string>[] { temp1, temp2 };
        }
    }
}
