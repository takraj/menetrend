using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFSConverter
{
    class GTFSReader
    {
        public static IEnumerable<T> Read<T>(string fileName) where T : class
        {
            var fileReader = System.IO.File.OpenText(fileName);
            var config = new CsvConfiguration();
            config.CultureInfo = CultureInfo.InvariantCulture;
            var csvReader = new CsvHelper.CsvReader(fileReader, config);
            return csvReader.GetRecords<T>();
        }
    }
}
