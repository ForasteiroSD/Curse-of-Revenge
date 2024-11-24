using UnityEngine;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class SaveSystem  {
    public static void SaveGame(GameManager gameManager) {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/game.curse";
        FileStream stream = new FileStream(path, FileMode.Create);

        GameData data = new GameData(gameManager);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static GameData LoadGame() {
        string path = Application.persistentDataPath + "/game.curse";
        if(File.Exists(path)) {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            GameData data = formatter.Deserialize(stream) as GameData;
            stream.Close();

            return data;
        }
        
        else {
            // Debug.LogError("Save file not found in " + path);
            return null;
        }
    }
}