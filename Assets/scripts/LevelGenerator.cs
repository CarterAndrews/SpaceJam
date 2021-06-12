using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    // Load image
    public Texture2D image;

    public GameObject white;
    public GameObject black;
    public GameObject red;
    public GameObject green;
    public GameObject blue;
    public GameObject cyan;
    public GameObject magenta;
    public GameObject yellow;

    void Start()
    {
        // Iterate through it's pixels
        for (int i = 0; i < image.width; i++)
        {
            for (int j = 0; j < image.height; j++)
            { 
                Color pixel = image.GetPixel(i, j);
                
                GameObject toPlace = null;

                if(pixel == Color.white)
                    toPlace = white;
                else if(pixel == Color.black)
                    toPlace = black;
                else if(pixel == Color.red)
                    toPlace = red;
                else if(pixel == Color.green)
                    toPlace = green;
                else if(pixel == Color.blue)
                    toPlace = blue;
                else if(pixel == Color.cyan)
                    toPlace = cyan;
                else if(pixel == Color.magenta)
                    toPlace = magenta;
                else if(pixel == Color.yellow)
                    toPlace = yellow;
                
                if(toPlace)
                    Instantiate(toPlace, new Vector3(i,0,j), Quaternion.identity).transform.parent = transform;
                
            }
        }
    }

}
