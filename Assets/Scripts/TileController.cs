using UnityEngine;
using System;

enum Direction
{
    Left, Right, Front, Back
}

public class TileController : MonoBehaviour
{
    [SerializeField] ColorController colorData;
    [SerializeField] CameraController cameraController;
    [SerializeField] Transform reference;
    [SerializeField] MeshRenderer referenceMesh;

    [SerializeField] GameObject fallingPrefab;
    [SerializeField] GameObject standPrefab;

    [SerializeField] Transform last;

    [SerializeField, Range(0, 5)] float speed;
    [SerializeField, Range(1, 2)] float limit;
    bool _isForward;
    bool _isAxisX;

    bool _isStop;

    int _score;
    [SerializeField] TMPro.TextMeshProUGUI scoreText;

    void UpdateScore()
    {
        _score++;
        scoreText.text = _score.ToString();
    }

    private void Start()
    {
        _score = 0;
        scoreText.text = _score.ToString();
    }

    private void LateUpdate()
    {
        if (_isStop) return;

        Vector3 position = transform.position;
        int direction = _isForward ? 1 : -1;
        float move = speed * Time.deltaTime * direction;

        //movement direction
        if (_isAxisX) position.x += move;
        else position.z += move;

        LimitNChange(position);

        transform.position = position;
    }

    void LimitNChange(Vector3 pos)
    {//Limit movement and change direction
        if (_isAxisX)
        {
            if (!(pos.x < -limit || pos.x > limit)) return; //I dont like nesting

            pos.x = Mathf.Clamp(pos.x, -limit, limit);
            _isForward = !_isForward;
            return;
        }  //else
        if (!(pos.z < -limit || pos.z > limit)) return; //I dont like nesting
        pos.z = Mathf.Clamp(pos.z, -limit, limit);
        _isForward = !_isForward;
    }

    void SplitObject(bool isAxisX, float value)
    {
        bool isFallingFirst = value > 0;

        Transform falling = Instantiate(fallingPrefab).transform;
        Transform stand = Instantiate(standPrefab).transform;

        //size
        Vector3 fallingSize = reference.localScale;
        if (isAxisX) fallingSize.x = Mathf.Abs(value);
        else fallingSize.z = Mathf.Abs(value);
        falling.localScale = fallingSize;

        Vector3 standSize = reference.localScale;
        if (isAxisX) standSize.x = reference.localScale.x - Mathf.Abs(value);
        else standSize.z = reference.localScale.z - Mathf.Abs(value);
        stand.localScale = standSize;

        Direction minDirection = isAxisX ? Direction.Left : Direction.Back;
        Direction maxDirection = isAxisX ? Direction.Right : Direction.Front;

        //position
        Vector3 fallingPosition = GetEdgePosition(referenceMesh, isFallingFirst ? minDirection : maxDirection);
        if (isAxisX) fallingPosition.x += (fallingSize.x / 2) * (isFallingFirst ? 1 : -1);
        else fallingPosition.z += (fallingSize.z / 2) * (isFallingFirst ? 1 : -1);
        falling.position = fallingPosition;


        Vector3 standPosition = GetEdgePosition(referenceMesh, !isFallingFirst ? minDirection : maxDirection);
        if (isAxisX) standPosition.x += (standSize.x / 2) * (!isFallingFirst ? 1 : -1);
        else standPosition.z += (standSize.z / 2) * (!isFallingFirst ? 1 : -1);
        stand.position = standPosition;

        //Color
        var color = colorData.GetColor(_score);
        stand.GetComponent<MeshRenderer>().material.color = color;
        falling.GetComponent<MeshRenderer>().material.color = color;
        referenceMesh.material.color = color;

        last = stand;
    }

    Vector3 GetEdgePosition(MeshRenderer mesh, Direction direction)
    {
        Vector3 extents = mesh.bounds.extents;
        Vector3 position = mesh.transform.position;

        switch (direction)
        {
            case Direction.Left:
                position.x -= extents.x;
                break;
            case Direction.Right:
                position.x += extents.x;
                break;
            case Direction.Front:
                position.z += extents.z;
                break;
            case Direction.Back:
                position.z -= extents.z;
                break;
        }

        return position;
    }

    public void OnClick()
    {
        _isStop = true;
        Vector3 distance = last.position - transform.position;

        if (isFail(distance))
        {
            Debug.Log("Game Over");
            return;
        }

        SplitObject(_isAxisX, _isAxisX ? distance.x : distance.z);
        SetNewTile();
        UpdateScore();
        cameraController.Up();
    }

    void SetNewTile()
    {
        _isAxisX = !_isAxisX;

        Vector3 newPosition = last.position;
        newPosition.y += transform.localScale.y;

        if (_isAxisX) newPosition.x = limit;
        else newPosition.z = limit;

        transform.position = newPosition;

        transform.localScale = last.localScale;
        _isStop = false;
    }

    bool isFail(Vector3 dist)
    {
        float origin = _isAxisX ? transform.localScale.x : transform.localScale.z;
        float current = _isAxisX ? Mathf.Abs(dist.x) : Mathf.Abs(dist.z);

        return current >= origin;
    }

}
