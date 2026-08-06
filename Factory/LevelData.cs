using System.Collections.Generic;

[System.Serializable]
public class AssetsData
{
    public Path paths;
    public List<AssetData> assets0;
    public List<AssetData> assets1;
    public List<AssetData> assets2;
}

[System.Serializable]
public class Path
{
    public List<string> plateV;
    public List<string> plateH;
    public List<string> items;
    public List<string> characters;
    public List<string> backgrounds;
    public List<string> decorations;
    public List<string> obstacles;
    public List<string> triggers;
    public List<string> walls;
    public List<string> bullets;
    public List<string> enemies;
    public List<string> gems;
    public List<string> bosses;
    public List<string> audios;
}
