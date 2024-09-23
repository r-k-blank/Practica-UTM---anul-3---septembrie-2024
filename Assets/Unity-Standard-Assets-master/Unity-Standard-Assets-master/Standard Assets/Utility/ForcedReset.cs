using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Include the UI namespace for using Image
using UnityStandardAssets.CrossPlatformInput;

// Update the RequireComponent to use Image instead of GUITexture
[RequireComponent(typeof(Image))]
public class ForcedReset : MonoBehaviour
{
    private void Update()
    {
        // If we have forced a reset...
        if (CrossPlatformInputManager.GetButtonDown("ResetObject"))
        {
            // Reload the scene
            SceneManager.LoadScene(SceneManager.GetSceneAt(0).name);
        }
    }
}
