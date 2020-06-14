using System.Collections.Generic;
using System.IO;
using MLScraper.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using Serilog;

namespace MLScraper.Helpers
{
    public class JSONSerializer
    {
        private static JSONSerializer instance = null;

        public static JSONSerializer Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new JSONSerializer();
                }
                return instance;
            }
        }

        public void Serialize(List<Category> categories, CategoryTypeEnum CategoryType)
        {

            Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
            serializer.Converters.Add(new JavaScriptDateTimeConverter());
            serializer.NullValueHandling = NullValueHandling.Ignore;
            serializer.Formatting = Formatting.Indented;

            try
            {
                using (StreamWriter sw = new StreamWriter(string.Format("{0}-{1}.json", CategoryType, DateTime.UtcNow.ToString("yyyy-MM-dd"))))
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    serializer.Serialize(writer, categories);
                }

                Log.Information("{CategoryType} successfully written to JSON file", CategoryType);
            }
            catch (Exception ex)
            {
                Log.Error("An error ocurred while trying to write the JSON file: {Ex}", ex);
            }
        }
    }
}