using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonVSBson
{
    class Program
    {
        static string experimentFolder = @"F:\experimentJSonVSBson\";
        static string file = @"F:\muzlome_CHVRCHS_-_Warning_Call_51841071.mp3";
        static string fileAsBson = @"F:\muzlome_CHVRCHS_-_Warning_Call_51841071.BSON";
        static string fileAsJson = @"F:\muzlome_CHVRCHS_-_Warning_Call_51841071.JSON";
        static string fileAsBsonDeserialized = @"F:\muzlome_CHVRCHS_-_Warning_Call_51841071.BSON-Deserialized";
        static string fileAsJsonDeserialized = @"F:\muzlome_CHVRCHS_-_Warning_Call_51841071.JSON-Deserialized";


        static void Main(string[] args)
        {
            byte[] byteArray = null;
            byteArray = File.ReadAllBytes(file);
            var prettifiedData = new Tuple<int, byte[]>(0, byteArray);

            Stopwatch sw = Stopwatch.StartNew();
            var dataASJson = JsonConvert.SerializeObject(byteArray);
            Console.WriteLine(sw.ElapsedMilliseconds);
            sw.Restart();
            File.WriteAllText(fileAsJson, dataASJson);


            using (MemoryStream memStream = new MemoryStream())
            {
                using (BsonDataWriter writer = new BsonDataWriter(memStream))
                {
                    using (FileStream fileStream = new FileStream(fileAsBson, FileMode.Create, FileAccess.Write))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        serializer.Serialize(writer, prettifiedData);
                        Console.WriteLine(sw.ElapsedMilliseconds);
                        memStream.Position = 0;
                        memStream.CopyTo(fileStream);
                    }
                }
            }

            Console.WriteLine("Чтение файла в виде Bson.");

            byteArray = File.ReadAllBytes(fileAsBson);
            sw.Restart();
            using (MemoryStream memStream = new MemoryStream(byteArray))
            {
                using (BsonDataReader writer = new BsonDataReader(memStream))
                {
                    using (FileStream fileStream = new FileStream(fileAsBsonDeserialized, FileMode.Create, FileAccess.Write))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        serializer.Deserialize<Tuple<int, byte[]>>(writer);
                        Console.WriteLine(sw.ElapsedMilliseconds);
                        memStream.Position = 0;
                        memStream.CopyTo(fileStream);
                    }
                }
            }

            Console.WriteLine("Чтение файла в виде Json.");

            var jsonFile = File.ReadAllText(fileAsJson);
            sw.Restart();
            byteArray = JsonConvert.DeserializeObject<byte[]>(jsonFile);
            Console.WriteLine(sw.ElapsedMilliseconds);

            File.WriteAllBytes(fileAsJsonDeserialized, byteArray);

            Console.ReadLine();
        }
    }
}
