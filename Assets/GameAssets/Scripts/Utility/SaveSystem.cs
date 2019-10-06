using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem<T> where T : class, ISavable, new() {
    public static void saveSettings(T saveObject, string fileName) {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/" + fileName;
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, saveObject);
        stream.Close();
    }

    public static T loadSettings(string fileName) {
        string path = Application.persistentDataPath + "/" + fileName;
        if (File.Exists(path)) {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            T save = formatter.Deserialize(stream) as T;
            stream.Close();
            save.load();
            
            return save;
        } else {
            T newS = new T();
            newS.empty();
            return newS;
        }
    }
}
