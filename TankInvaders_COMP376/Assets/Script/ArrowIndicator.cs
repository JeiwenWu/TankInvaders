using UnityEngine;
using UnityEngine.UI;

public class ArrowIndicator : MonoBehaviour
{
    public Transform player; // Reference to the player's Transform
    public Transform gasCloud; // Reference to the gas cloud's Transform
    public Image arrowImage; // Reference to the arrow Image

    void Update()
    {
        // Calculate direction from the player's turret to the gas cloud
        Vector3 directionToGasCloud = gasCloud.position - player.Find("TopPart/Turret").position;
        directionToGasCloud.y = 0; // Ignore vertical direction

        // Calculate the angle between the player's turret forward direction and the gas cloud
        float angle = Vector3.SignedAngle(player.Find("TopPart/Turret").forward, directionToGasCloud, Vector3.up);

        // Rotate the arrow image to point towards the gas cloud
        arrowImage.rectTransform.rotation = Quaternion.Euler(0, 0, -angle);
    }
}
