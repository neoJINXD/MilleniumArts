using UnityEngine;
using Zone.Core.Utils;

/*
    Script that holds info for network connection settings
*/
public class NetworkSettings : Singleton<NetworkSettings>
{
    [SerializeField] private string _gameVersion = "0.0.0";
    public string GameVersion { get { return _gameVersion; } }

    [SerializeField] private string _nickname = "MilleniumArts";
    public string Nickname { get { return $"{_nickname}{Random.Range(0,5)}"; } }
}
