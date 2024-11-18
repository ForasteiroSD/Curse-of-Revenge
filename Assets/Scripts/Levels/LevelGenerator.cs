using System.Collections;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private Transform[] _upperPatterns;
    [SerializeField] private Transform[] _descendingPatterns;
    [SerializeField] private Transform[] _underGroundPatterns;
    [SerializeField] private Transform[] _underGroundDescendingPatterns;
    [SerializeField] private Transform[] _underGroundLastPatterns;
    [SerializeField] private Transform _lastPattern;
    [SerializeField] private int _mapSize;
    [SerializeField] private int _belowLevelSizeDivisor = 3;
    private List<TilemapCollider2D> _instatiatedPatterns = new List<TilemapCollider2D>();
    private Transform _pattern;
    private Vector3 _lastEndPosition;

    private void Awake() {
        _lastEndPosition = transform.position;
        int descendingRoom = UnityEngine.Random.Range(0, _mapSize);
        Transform lastPattern;

        //Spawn all upper patterns
        for(int i = 0; i < _mapSize; i++) {
            if(i == descendingRoom) {
                _pattern = _descendingPatterns[UnityEngine.Random.Range(0, _descendingPatterns.Length)];
                lastPattern = SpawnDescendingLevelPart(_pattern, _lastEndPosition, Quaternion.Euler(0f, 0f, 0f), 1);
            }
            else {
                _pattern = _upperPatterns[UnityEngine.Random.Range(0, _upperPatterns.Length)];
                lastPattern = Instantiate(_pattern, _lastEndPosition, Quaternion.identity);
            }

            _lastEndPosition = lastPattern.Find("EndPosition").position;
        }
        
        //Spawn last pattern
        _pattern = _lastPattern;
        Instantiate(_pattern, _lastEndPosition, Quaternion.identity);
    }

    private Transform SpawnDescendingLevelPart(Transform pattern, Vector3 spawnPosition, Quaternion rotation, int nivel) {
        Transform newPattern = Instantiate(pattern, spawnPosition, rotation);

        //Ramdomly choses one side to be the descending side
        bool isDescendingSide = Random.value < 0.5f;

        //Check if it's already mirrored (to unmiror)
        bool isMirrored = rotation.y > 0;

        if(newPattern.Find("LeftEnd")) SpawnBelowLevelPart(newPattern, "LeftEnd", Quaternion.Euler(0f, (isMirrored ? 0f : 180f), 0f), nivel, isDescendingSide);
        if(newPattern.Find("RightEnd")) SpawnBelowLevelPart(newPattern, "RightEnd", Quaternion.Euler(0f, (isMirrored ? 180f : 0f), 0f), nivel, !isDescendingSide);
        return newPattern;
    }

    private void SpawnBelowLevelPart(Transform lastPattern, string side, Quaternion rotation, int nivel, bool isDescendingSide) {
        Vector3 spawnPosition;
        int descendingRoom = -1;

        //the size of this side of the level is going to be at max 1/_belowLevelSizeDivisor the size of the upper level
        int size = UnityEngine.Random.Range(0, _mapSize/(_belowLevelSizeDivisor*nivel));

        //Ramdomly chooses one of the patterns to be de descending pattern
        if(isDescendingSide && size > 0) descendingRoom = UnityEngine.Random.Range(0, size);

        spawnPosition = lastPattern.Find(side).position;

        //Spawn the patterns
        for(int i = 0; i <= size; i++) {
            if(i == descendingRoom) {
                _pattern = _underGroundDescendingPatterns[UnityEngine.Random.Range(0, _underGroundDescendingPatterns.Length)];
                lastPattern = SpawnDescendingLevelPart(_pattern, spawnPosition, rotation, nivel+1);
                spawnPosition = lastPattern.Find("EndPosition").position;
            }
            else {
                _pattern = _underGroundPatterns[UnityEngine.Random.Range(0, _underGroundPatterns.Length)];
                lastPattern = Instantiate(_pattern, spawnPosition, rotation);
                spawnPosition = lastPattern.Find("EndPosition").position;
            }
        }

        //Spawn the closing pattern
        _pattern = _underGroundLastPatterns[UnityEngine.Random.Range(0, _underGroundLastPatterns.Length)];
        Instantiate(_pattern, spawnPosition, rotation);
    }
}
