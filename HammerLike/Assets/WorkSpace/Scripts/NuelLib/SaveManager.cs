using System.IO;
using UnityEngine;

// Check if Newtonsoft.Json package is installed
#if NEWTONSOFT_INSTALLED
using Newtonsoft.Json;
#endif

namespace NuelLib.Utils.Save
{
    public class SaveManager : MonoBehaviour
    {
        public static void Save<T>(T data, string path, string fileName = "")
        {
#if NEWTONSOFT_INSTALLED
            if (fileName == "")
            {
                fileName = Time.realtimeSinceStartup + "_" + "SaveData" + ".save";
            }

            var filePath = Application.persistentDataPath + "/" + path + "/" + fileName;
            Debug.Log(filePath);

            // Check if Newtonsoft.Json package is installed
            bool isNewtonsoftJsonInstalled = DependencyChecker.CheckDependencies("Newtonsoft.Json");

            if (isNewtonsoftJsonInstalled)
            {
                // Serialize the data to JSON using Newtonsoft.Json
                var json = JsonConvert.SerializeObject(data);

                // Ensure the directory exists
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                // Write the JSON data to the file
                File.WriteAllText(filePath, json);
                Debug.Log("JSON DATA : " + json.ToString());
            }
            else
            {
                Debug.LogError("Newtonsoft.Json package is not installed. Unable to serialize data.");
            }
#else
            Debug.LogError("Newtonsoft.Json package is not installed. Unable to serialize data.");
#endif
        }

        public static T Load<T>(string path, string fileName = "")
        {
            var filePath = Application.persistentDataPath + "/" + path;
            var saveLoc = Path.Combine(filePath + "/" + fileName);
#if NEWTONSOFT_INSTALLED
            if (File.Exists(saveLoc))
            {
                // Check if Newtonsoft.Json package is installed
                bool isNewtonsoftJsonInstalled = DependencyChecker.CheckDependencies("Newtonsoft.Json");

                if (isNewtonsoftJsonInstalled)
                {
                    // Read the JSON data from the file
                    string json = File.ReadAllText(saveLoc);

                    // Deserialize the JSON data using Newtonsoft.Json
                    T loadedData = JsonConvert.DeserializeObject<T>(json);

                    return loadedData;
                }
                else
                {
                    Debug.LogError("Newtonsoft.Json package is not installed. Unable to deserialize data.");
                    return default;
                }
            }
            else
            {
                Debug.LogWarning("Save file not found.");
                return default;
            }
#else
            Debug.LogError("Newtonsoft.Json package is not installed. Unable to deserialize data.");
            return default;
#endif
        }
    }

}