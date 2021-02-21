using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AutoMLDataReader.Core
{
    public class DataLoader
    {

        public async Task<DataTable> LoadData(string csvPath,string endpointUrl,string key)
        {
            var dt=loadCSV(csvPath);
            var x = genDictFromDataTable(dt).ToList();
            EndpointClient client = new EndpointClient(endpointUrl);
            client.APIKey = key;
            var s=await client.InvokeRequestResponseService(x);
            string r1 = JsonConvert.DeserializeObject<string>(s);
            var tmp = JsonConvert.DeserializeObject<AMLPredictResult>(r1);
            DataTable result = new DataTable();
            foreach (var item in tmp.index[0])
            {
                result.Columns.Add(item.Key, item.Value is long ? typeof(DateTime) : typeof(string));
            }
            result.Columns.Add("predict", typeof(double));


            for (int i = 0; i < tmp.forecast.Count; i++)
            {
                DataRow row = result.NewRow();
                foreach (var item in tmp.index[i])
                {
                    if (item.Value is long)
                    {
                        row[item.Key] = DateTime.UnixEpoch.AddMilliseconds((long)item.Value);
                    }
                    else
                    {
                        row[item.Key] = item.Value.ToString();
                    }
                }
                row["predict"] = tmp.forecast[i];
                result.Rows.Add(row);
            }
            
            return result;
        }

        private IEnumerable<Dictionary<string,string>> genDictFromDataTable(DataTable dt)
        {
            foreach (DataRow row in dt.Rows)
            {
                Dictionary<string, string> item = new Dictionary<string, string>();
                foreach (DataColumn c in dt.Columns)
                {
                    item.Add(c.ColumnName, row.Field<string>(c));
                }
                yield return item;
            }
        }

        private DataTable loadCSV(string csvPath)
        {
            using (var reader = new StreamReader(csvPath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                // Do any configuration to `CsvReader` before creating CsvDataReader.
                using (var dr = new CsvDataReader(csv))
                {
                    var dt = new DataTable();
                    dt.Load(dr);
                    return dt;
                }
            }
        }
    }
}
