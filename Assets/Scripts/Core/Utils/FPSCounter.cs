using UnityEngine;
using UnityEngine.UI;
// using TMPro;

// Author: Anik Patel
// Description: Basic class for use as a UI FPS counter
// Version: 1.0
// Changes: [N/A]
namespace Zone.Core.Utils
{
    public class FPSCounter : MonoBehaviour
    {
        // Refences to the Text Components
        [SerializeField] private Text fpsText = null;

        private int fps;

        void Update()
        {
            fps = (int) (1f / Time.unscaledDeltaTime);

            fpsText.text = $"{fps} fps";
            // Debug.Log($"Running at {fps}");
        }
    }
}
