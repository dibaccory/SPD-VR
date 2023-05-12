using System;
using UnityEngine;
using DungeonInstance;


public class DungeonGenerator : MonoBehaviour
{
	public Dungeon dungeon;

    void Awake()
    {
        Console.WriteLine("YAWN");
    }

	void Start()
	{
        Console.WriteLine("did we get here ");
        dungeon = new();

        //Console.WriteLine(dungeon.level.map);
    }

}
