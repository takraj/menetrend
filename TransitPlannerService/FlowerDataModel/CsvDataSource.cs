using DataAccess;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FlowerDataModel
{
    public class CsvDataSource : IDataSource
    {
        private string rootDirectory;

        public CsvDataSource(string rootDirectory)
        {
            this.rootDirectory = rootDirectory;
        }

        protected Dictionary<string, string> CreateDictionary(Row row)
        {
            var result = new Dictionary<string, string>();
            var headers = row.ColumnNames.ToArray();
            var values = row.Values.ToArray();

            for (int i = 0; i < headers.Length; i++)
            {
                var key = headers[i];
                var value = values[i];
                result[key] = value;
            }

            return result;
        }

        protected IEnumerable<Dictionary<string, string>> ReadCSV(string path)
        {
            var csv = DataTable.New.ReadCsv(path);
            foreach (var row in csv.Rows)
            {
                yield return CreateDictionary(row);
            }
        }

        public IEnumerable<Dictionary<string, string>> GetAllMetaInfo()
        {
            var path = Path.Combine(this.rootDirectory, "metadata.csv");
            return this.ReadCSV(path);
        }

        public IEnumerable<Dictionary<string, string>> GetAllRoute()
        {
            var path = Path.Combine(this.rootDirectory, "routes.csv");
            return this.ReadCSV(path);
        }

        public IEnumerable<Dictionary<string, string>> GetAllSchedule()
        {
            var path = Path.Combine(this.rootDirectory, "schedules.csv");
            return this.ReadCSV(path);
        }

        public IEnumerable<Dictionary<string, string>> GetAllSequenceLookupData()
        {
            var path = Path.Combine(this.rootDirectory, "sequence_lookup.csv");
            return this.ReadCSV(path);
        }

        public IEnumerable<Dictionary<string, string>> GetAllSequence()
        {
            var path = Path.Combine(this.rootDirectory, "sequences.csv");
            return this.ReadCSV(path);
        }

        public IEnumerable<Dictionary<string, string>> GetAllTransferByWalkInfo()
        {
            var path = Path.Combine(this.rootDirectory, "stop_distances.csv");
            return this.ReadCSV(path);
        }

        public IEnumerable<Dictionary<string, string>> GetAllStopRoutePair()
        {
            var path = Path.Combine(this.rootDirectory, "stop_routes.csv");
            return this.ReadCSV(path);
        }

        public IEnumerable<Dictionary<string, string>> GetAllStop()
        {
            var path = Path.Combine(this.rootDirectory, "stops.csv");
            return this.ReadCSV(path);
        }

        public IEnumerable<Dictionary<string, string>> GetAllTrip()
        {
            var path = Path.Combine(this.rootDirectory, "trips.csv");
            return this.ReadCSV(path);
        }
    }
}
