using System.Collections;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private Transform[] _patterns;
    [SerializeField] private Transform[] _underGroundPatterns;
    [SerializeField] private Transform _lastPattern;
    [SerializeField] private int _mapSize;
    private List<MeshCollider> _instatiatedPatterns = new List<MeshCollider>();
    private Transform _pattern;
    private Transform _startPosition;
    private Vector3 _lastEndPosition;

    private void Awake() {
        _startPosition = GameObject.Find("StartPosition").transform;
        _lastEndPosition = _startPosition.position;
        int caveRoom = UnityEngine.Random.Range(0, _mapSize);
        Transform lastPattern;

        for(int i = 0; i < _mapSize; i++) {
            _pattern = _patterns[UnityEngine.Random.Range(0, _patterns.Length)];

            lastPattern = SpawnLevelPart(_lastEndPosition);
            _lastEndPosition = lastPattern.Find("EndPosition").position;
        }
        
        _pattern = _lastPattern;
        SpawnLevelPart(_lastEndPosition);
    }

    private Transform SpawnLevelPart(Vector3 spawnPosition) {
        spawnPosition += new Vector3(_pattern.GetComponent<MeshCollider>().bounds.extents.x, 0, 0);
        Transform newPattern = Instantiate(_pattern, spawnPosition, Quaternion.identity);
        if(newPattern.Find("LeftEnd")) SpawnBelowLevelPart(newPattern, "LeftEnd", Quaternion.Euler(0f, 180f, 0f));
        if(newPattern.Find("RightEnd")) SpawnBelowLevelPart(newPattern, "RightEnd", Quaternion.identity);
        // _instatiatedPatterns.Append(newPattern);
        return newPattern;
    }

    private void SpawnBelowLevelPart(Transform lastPattern, string side, Quaternion rotarion) {
        Vector3 spawnPosition;

        //Spawn first pattern
        spawnPosition = lastPattern.Find(side).position;
        _pattern = _underGroundPatterns[UnityEngine.Random.Range(0, _underGroundPatterns.Length)];
        lastPattern = Instantiate(_pattern, spawnPosition, rotarion);

        CheckCollision(lastPattern);

        //Continue spawning the patterns until ramdomly reaches end
        while(lastPattern.Find("EndPosition")) {
            spawnPosition = lastPattern.Find("EndPosition").position;
            _pattern = _underGroundPatterns[UnityEngine.Random.Range(0, _underGroundPatterns.Length)];

            lastPattern = Instantiate(_pattern, spawnPosition, rotarion);

            CheckCollision(lastPattern);
        }
    }

    private void CheckCollision(Transform pattern) {
        print(_instatiatedPatterns.Count);
        bool hadCollision = false;
        // _instatiatedPatterns.Add(pattern.GetComponent<MeshCollider>());

        // print(pattern.Find("Grid").Find("ForeGround"));
        for (int i = 0; i < _instatiatedPatterns.Count; i++) {
            if(pattern.GetComponent<MeshCollider>().bounds.Intersects(_instatiatedPatterns[i].bounds)) {
                Destroy(pattern.gameObject);
                hadCollision = true;
                print("entrei aqui em");
                // return;
            }
        }

        if(!hadCollision) _instatiatedPatterns.Add(pattern.GetComponent<MeshCollider>());
        // if(pattern.Find("Grid").Find("ForeGround").GetComponent<TilemapCollider2D>().IsTouchingLayers(LayerMask.GetMask("Ground"))) {
        //     Destroy(pattern.gameObject);
        //     print("entrei aqui em");
        // }
    }

    // private void Awake() {
    //     _startPosition = GameObject.Find("StartPosition").transform;
    //     _lastEndPosition = _startPosition.position;
    //     for(int i = 0; i < _mapSize - 1; i++) {
    //         if(i != _mapSize - 2) _pattern = _patterns[UnityEngine.Random.Range(0, _patterns.Length - 2)];
    //         else _pattern = _patterns[_patterns.Length - 1];
    //         _lastEndPosition = SpawnLevelPart(_lastEndPosition);
    //     }
    // }

    // private Vector3 SpawnLevelPart(Vector3 spawnPosition) {
    //     print(new Vector3(_pattern.GetComponent<MeshCollider>().bounds.extents.x, 0, 0));
    //     spawnPosition += new Vector3(_pattern.GetComponent<MeshCollider>().bounds.extents.x, 0, 0);
    //     print(spawnPosition);
    //     Transform newPattern = Instantiate(_pattern, spawnPosition, Quaternion.identity);
    //     return spawnPosition;
    // }
}
