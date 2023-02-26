using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public string _playerName;
    public float _waveNum;
    public int[] _killedEnemies;

    public PlayerData()
    {
        _playerName = WaveController.PLAYER_NAME;
        _waveNum = WaveController.waveNum;
        _killedEnemies = new int[WaveController.killedEnemies.Count];
        for(int i = 0; i < WaveController.killedEnemies.Count; i++)
        {
            _killedEnemies[i] = WaveController.killedEnemies[i];
        }
    }
}
