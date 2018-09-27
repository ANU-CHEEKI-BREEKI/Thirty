using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{

    Squad squadForFollow;

    Transform thisTransform;

    float defaultCameraSize;
   
    [SerializeField] [Range(0, 100)] float aceleration = 1;
    [SerializeField] [Range(0, 1)] float acurasy = 1;

    [SerializeField] [Range(0, 100)] float maxSpeed;

    Camera thisCam;

    [Space]

    [SerializeField] float currentSpeed;

	void Start ()
    {
        thisTransform = transform;
        squadForFollow = GameObject.FindWithTag("Player").GetComponent<Squad>();

        thisCam = GetComponent<Camera>();

        defaultCameraSize = thisCam.orthographicSize;
    }
	
	void Update ()
    {
        if (squadForFollow != null)
        {
            if (Vector2.SqrMagnitude((Vector2)thisTransform.position - squadForFollow.CenterSquad) > acurasy * acurasy)
            {
                if (currentSpeed < maxSpeed)
                    currentSpeed += aceleration * Time.deltaTime;
                else
                    currentSpeed = maxSpeed;
            }
            else
            {
                currentSpeed = 0;
            }

            Vector3 newPosition = Vector3.MoveTowards(thisTransform.position, squadForFollow.CenterSquad, currentSpeed * Time.deltaTime);

            //что бы не выходила за пределы карты, делаем соответствующую проверку
            var mapWidth = Ground.Instance.ColCountOfBlocks * MapBlock.WORLD_BLOCK_SIZE;
            var mapHeight = Ground.Instance.RowCountOfBlocks * MapBlock.WORLD_BLOCK_SIZE;

            Vector2 scrSizeDivTwo = new Vector2(thisCam.orthographicSize * Screen.width / Screen.height, thisCam.orthographicSize);

            if (newPosition.x - scrSizeDivTwo.x < 0)
                newPosition.x = scrSizeDivTwo.x;
            else if (newPosition.x + scrSizeDivTwo.x > mapWidth)
                newPosition.x = mapWidth - scrSizeDivTwo.x;

            if (newPosition.y - scrSizeDivTwo.y < 0)
                newPosition.y = scrSizeDivTwo.y;
            else if (newPosition.y + scrSizeDivTwo.y > mapHeight)
                newPosition.y = mapHeight - scrSizeDivTwo.y;

            thisTransform.position = new Vector3(newPosition.x, newPosition.y, thisTransform.position.z);
        }
    }

    public void OnScrolbarValueChanged(float newValue)
    {
        thisCam.orthographicSize = defaultCameraSize + newValue;
        if (squadForFollow != null)
            thisTransform.position = new Vector3(squadForFollow.CenterSquad.x, squadForFollow.CenterSquad.y, thisTransform.position.z);
    }
}
