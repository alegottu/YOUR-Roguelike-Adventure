﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public GameObject layoutRoom;
    public Transform generatorPoint;
    public LayerMask whatIsRoom;
    public RoomPrefabs rooms;
    public RoomCenter centerStart, centerEnd;
    public RoomCenter[] potentialCenters;

    public enum Direction { up, right, down, left};
    [HideInInspector] public Direction selectedDirection;

    public int distanceToEnd = 5;
    public float xOffset = 26f, yOffset = 10f; // x: room length, y: room height

    [SerializeField] private GameObject endRoom;
    [SerializeField] private List<GameObject> layoutRoomObjects = new List<GameObject>();
    [SerializeField] private List<GameObject> generatedOutlines = new List<GameObject>();

    void Start()
    {
        Instantiate(layoutRoom, generatorPoint.position, generatorPoint.rotation);

        for (int i = 0; i < distanceToEnd; i++)
        {
            selectedDirection = (Direction)Random.Range(0, 4);
            MoveGenerationPoint();

            while (Physics2D.OverlapCircle(generatorPoint.position, .2f, whatIsRoom))
            {
                MoveGenerationPoint();
            }

            GameObject newRoom = Instantiate(layoutRoom, generatorPoint.position, generatorPoint.rotation);
            layoutRoomObjects.Add(newRoom);

            if (i + 1 == distanceToEnd)
            {
                layoutRoomObjects.RemoveAt(layoutRoomObjects.Count - 1);
                endRoom = newRoom;
            }
        }

        // Create room outlines
        CreateRoomOutline(Vector3.zero);
        foreach (GameObject room in layoutRoomObjects)
        {
            CreateRoomOutline(room.transform.position);
        }
        CreateRoomOutline(endRoom.transform.position);


        foreach (GameObject outline in generatedOutlines)
        {
            Vector3 centerPosition = new Vector3(outline.transform.position.x, outline.transform.position.y, outline.transform.position.z + 1);
            RoomCenter currentCenter = null;

            if (outline.transform.position == Vector3.zero)
            {
                currentCenter = Instantiate(centerStart, centerPosition, transform.rotation);
            }
            else if (outline.transform.position == endRoom.transform.position)
            {
                currentCenter = Instantiate(centerEnd, centerPosition, transform.rotation);
            }
            else
            {
                int centerSelect = Random.Range(0, potentialCenters.Length);
                currentCenter = Instantiate(potentialCenters[centerSelect], centerPosition, transform.rotation);
            }

            currentCenter.theRoom = outline.GetComponent<Room>();
            currentCenter.activator.doors = currentCenter.theRoom.doors;
        }
    }

    public void MoveGenerationPoint()
    {
        switch (selectedDirection)
        {
            case Direction.up:
                generatorPoint.position += new Vector3(0f, yOffset, 0f);
                break;

            case Direction.down:
                generatorPoint.position += new Vector3(0f, -yOffset, 0f);
                break;

            case Direction.right:
                generatorPoint.position += new Vector3(xOffset, 0f, 0f);
                break;

            case Direction.left:
                generatorPoint.position += new Vector3(-xOffset, 0f, 0f);
                break;
        }
    }

    public void CreateRoomOutline(Vector3 roomPosition)
    {
        bool roomAbove = Physics2D.OverlapCircle(roomPosition + new Vector3(0f, yOffset, 0f), .2f, whatIsRoom);
        bool roomBelow = Physics2D.OverlapCircle(roomPosition + new Vector3(0f, -yOffset, 0f), .2f, whatIsRoom);
        bool roomLeft = Physics2D.OverlapCircle(roomPosition + new Vector3(-xOffset, 0f, 0f), .2f, whatIsRoom);
        bool roomRight = Physics2D.OverlapCircle(roomPosition + new Vector3(xOffset, 0f, 0f), .2f, whatIsRoom);

        int directionCount = 0;
        if (roomAbove)
        {
            directionCount++;
        }
        if (roomBelow)
        {
            directionCount++;
        }
        if (roomLeft)
        {
            directionCount++;
        }
        if (roomRight)
        {
            directionCount++;
        }

        switch (directionCount)
        {
            case 0:
                Debug.LogError("How did this happen.");
                break;

            case 1:
                if (roomAbove)
                {
                    generatedOutlines.Add(Instantiate(rooms.singleUp, roomPosition, transform.rotation));
                }
                else if (roomBelow)
                {
                    generatedOutlines.Add(Instantiate(rooms.singleDown, roomPosition, transform.rotation));
                }
                else if (roomLeft)
                {
                    generatedOutlines.Add(Instantiate(rooms.singleLeft, roomPosition, transform.rotation));
                }
                else
                {
                    generatedOutlines.Add(Instantiate(rooms.singleRight, roomPosition, transform.rotation));
                }
                break;

            case 2:
                if (roomAbove && roomBelow)
                {
                    generatedOutlines.Add(Instantiate(rooms.doubleUpDown, roomPosition, transform.rotation));
                }
                else if (roomLeft && roomRight)
                {
                    generatedOutlines.Add(Instantiate(rooms.doubleLeftRight, roomPosition, transform.rotation));
                }
                else if (roomAbove && roomRight)
                {
                    generatedOutlines.Add(Instantiate(rooms.doubleUpRight, roomPosition, transform.rotation));
                }
                else if (roomBelow && roomRight)
                {
                    generatedOutlines.Add(Instantiate(rooms.doubleDownRight, roomPosition, transform.rotation));
                }
                else if (roomBelow && roomLeft)
                {
                    generatedOutlines.Add(Instantiate(rooms.doubleDownLeft, roomPosition, transform.rotation));
                }
                else
                {
                    generatedOutlines.Add(Instantiate(rooms.doubleLeftUp, roomPosition, transform.rotation));
                }
                break;

            case 3:
                if (!roomLeft)
                {
                    generatedOutlines.Add(Instantiate(rooms.tripleUpRightDown, roomPosition, transform.rotation));
                }
                else if (!roomAbove)
                {
                    generatedOutlines.Add(Instantiate(rooms.tripleRightDownLeft, roomPosition, transform.rotation));
                }
                else if (!roomRight)
                {
                    generatedOutlines.Add(Instantiate(rooms.tripleDownLeftUp, roomPosition, transform.rotation));
                }
                else
                {
                    generatedOutlines.Add(Instantiate(rooms.tripleLeftUpRight, roomPosition, transform.rotation));
                }
                break;

            case 4:
                generatedOutlines.Add(Instantiate(rooms.fourway, roomPosition, transform.rotation));
                break;
        }
    }
}

[System.Serializable]
public class RoomPrefabs
{
    public GameObject singleUp, singleDown, singleRight, singleLeft,
        doubleUpDown, doubleLeftRight, doubleUpRight, doubleDownRight, doubleDownLeft, doubleLeftUp, 
        tripleUpRightDown, tripleRightDownLeft, tripleDownLeftUp, tripleLeftUpRight,
        fourway;

}
