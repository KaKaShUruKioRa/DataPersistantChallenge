using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public string playerPseudo, bestPlayerPseudo;
    public int playerScore, bestScore;

    private void Start()
    {
    }
    // Start is called before the first frame update
    void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }
        
        instance = this;
        DontDestroyOnLoad(instance);
    }
    public void DefineBestScoreAndPLayer()
    {
        if (playerScore > bestScore)
        {
            bestScore = playerScore;
            bestPlayerPseudo = playerPseudo;

            SaveSrializableData();
        }
    }
    public void StartGame()
    {
        SceneManager.LoadScene("Main");
    }
    public void GameOver()
    {
        DefineBestScoreAndPLayer();
        SceneManager.LoadScene("GameOver");
    }
    public void BackToStartMenu()
    {
        SceneManager.LoadScene("StartMenu");
    }
    public void QuitApplication()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }

    [Serializable] 
    class SaveSerializableData
    {
        public static string nameSaveFile { get; } = "\\GameManagerDataFile.json";

        public string bestPlayerPseudo;
        public int bestScore;
    }

    public void SaveSrializableData()
    {
        SaveSerializableData data = new();
        data.bestPlayerPseudo = bestPlayerPseudo;
        data.bestScore = bestScore;
        
        string json = JsonUtility.ToJson(data);
        
        if (json != null)
        {
            string path = Application.persistentDataPath + SaveSerializableData.nameSaveFile; 

            File.WriteAllText(path, json);
        }
    }
    public void LoadSerializableData()
    {
        string path = Application.persistentDataPath + SaveSerializableData.nameSaveFile;
        if (path != null)
        {
            string json = File.ReadAllText(path);

            SaveSerializableData data = JsonUtility.FromJson<SaveSerializableData>(json);

            bestPlayerPseudo = data.bestPlayerPseudo;
            bestScore = data.bestScore;
        }
    }
}
