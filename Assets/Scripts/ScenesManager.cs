using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections.Generic;

public class ScenesManager : MonoBehaviour {

    [SerializeField]
    List<MapElement> mapElements = new List<MapElement>();

    [SerializeField]
    private GameObject playerController;

    [SerializeField]
    private int mapWidth = 500;
    [SerializeField]
    private int mapHeight = 500;

    private string[] scenePaths;

    private bool sceneSettingFinish = false;
	// Use this for initialization
	void Start () {

        List<string> sceneList = new List<string>();

        foreach (MapElement element in mapElements)
            sceneList.Add(string.Format("Assets/Scenes/Additives/{0}.unity", element.scene_name));

        SetEditorBuildSettingsScenes(sceneList.ToArray());
        
    }


    void Update()
    {
        MapElement currentElement = GetControllerCurrentMapElement();
        ActivateSceneByName(currentElement.scene_name);

        if(sceneSettingFinish)
            LoadNextMaps(currentElement);
    }

    private void ActivateSceneByName(string name)
    {
        Scene scene = SceneManager.GetSceneByName(name);

        Scene activeScene = SceneManager.GetActiveScene();

        if(activeScene.name != scene.name)
        {
            SceneManager.SetActiveScene(scene);
        }
    }

    private void LoadSceneByName(string name)
    {
        Scene scene = SceneManager.GetSceneByName(name);

        if (!scene.isLoaded)
        {
            SceneManager.LoadScene(name, LoadSceneMode.Additive);
        }
    }

    private void UnloadSceneByName(string name)
    {
        Scene scene = SceneManager.GetSceneByName(name);

        if (scene.isLoaded)
        {
            SceneManager.UnloadSceneAsync(name);
        }
    }

    private void LoadNextMaps(MapElement currentMapElement)
    {
        double worldX = playerController.transform.position.x;
        double worldZ = playerController.transform.position.z;

        int mapX = int.Parse(((double)worldX / (double)mapWidth).ToString().Split('.')[0]);
        int mapZ = int.Parse(((double)worldZ / (double)mapHeight).ToString().Split('.')[0]);

        double diffX = (worldX - ((double)mapX * (double)mapWidth));
        double diffZ = (worldZ - ((double)mapZ * (double)mapHeight));

        if ((int)(diffX) == 0)
            return;

        if ((int)(diffZ) == 0)
            return;
        
        int nextX = (diffX > ((double)mapWidth / 2f)) ? mapX +1: mapX - 1;
        int nextZ = (diffZ > ((double)mapHeight / 2f)) ? mapZ + 1 : mapZ - 1;
        
        foreach(MapElement element in mapElements)
        {
            if (element.scene_name == currentMapElement.scene_name)
                continue;

            //Debug.Log(string.Format("nextX :{0} - nextZ : {1} - diff x : {2} - diff z : {3}", nextX, nextZ, diffX, diffZ));
            if(element.x == nextX && (element.z == mapZ || element.z == nextZ) 
                || element.z == nextZ && (element.x == mapX || element.x == nextX))
            {
                LoadSceneByName(element.scene_name);
            }
            else
            {
                UnloadSceneByName(element.scene_name);
            }
        }
    }

    private MapElement GetControllerCurrentMapElement()
    {
        int worldX = (int)playerController.transform.position.x;
        int worldZ = (int)playerController.transform.position.z;

        int mapX = int.Parse( ((double)worldX / (double)mapWidth).ToString().Split('.')[0]);
        int mapZ = int.Parse(((double)worldZ / (double)mapHeight).ToString().Split('.')[0]);

        Debug.Log(string.Format("{0} - {1}", mapX, mapZ));
        return GetMapElementByPosition(mapX, mapZ);
    }

    private MapElement GetMapElementByName(string name)
    {
        foreach(MapElement element in mapElements)
        {
            if(element.scene_name == name)
            {
                return element;
            }
        }

        return null;
    }

    public MapElement GetMapElementByPosition(int x, int z)
    {
        foreach (MapElement element in mapElements)
        {
            if (element.x == x && element.z == z)
            {
                return element;
            }
        }

        return null;
    }

    public void SetEditorBuildSettingsScenes(string[] scenePaths)
    {
        
        // Find valid Scene paths and make a list of EditorBuildSettingsScene
        List<EditorBuildSettingsScene> editorBuildSettingsScenes = new List<EditorBuildSettingsScene>();
        foreach(string scenePath in scenePaths)
        {
            EditorBuildSettingsScene sceneBuildSetting = new EditorBuildSettingsScene(scenePath, true);
            if (!editorBuildSettingsScenes.Contains(sceneBuildSetting))
                editorBuildSettingsScenes.Add(sceneBuildSetting);
        }        
        
        // Set the Build Settings window Scene list
        EditorBuildSettings.scenes = editorBuildSettingsScenes.ToArray();

        sceneSettingFinish = true;
    }

}
