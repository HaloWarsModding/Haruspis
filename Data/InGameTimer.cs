using System.Xml.Serialization;

namespace Data
{
    public class InGameTimer
    {
        #region Properties
        public Timers Data { get; set; } = new Timers();

        public class Timers
        {
            public TimeSpan PlayTime { get; set; } = TimeSpan.Zero;
            public DateTime LastPlayed { get; set; } = DateTime.MinValue;
        }
        #endregion

        public void ToFile(string path)
        {
            try
            {
                using (StreamWriter streamWriter = new(path))
                {
                    XmlSerializer xmlSerializer = new(typeof(Timers));
                    xmlSerializer.Serialize(streamWriter, Data);
                }

                Console.WriteLine("Data file successfully written to: " + path);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error writing data file: " + ex.Message);
            }
        }

        public void FromFile(string path)
        {
            if (!DataFileExists(path))
            {
                Console.WriteLine("Data file does not exist: " + path);
                return;
            }

            try
            {
                using (StreamReader streamReader = new(path))
                {
                    XmlSerializer xmlSerializer = new(typeof(Timers));
                    Data = (Timers)xmlSerializer.Deserialize(streamReader);
                }

                Console.WriteLine($"Data file successfully read from: {path}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading data file: " + ex.Message);
            }
        }

        private static bool DataFileExists(string path)
        {
            return File.Exists(path);
        }
    }
}
