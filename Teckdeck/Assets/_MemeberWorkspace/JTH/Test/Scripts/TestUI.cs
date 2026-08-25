using TMPro;
using UnityEngine;

namespace JTH.Test.Scripts
{
    public class TestUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI speedUI;
        [SerializeField] private Rigidbody rb;

        private void Update()
        {
            speedUI.text = "Speed: " + rb.linearVelocity.magnitude;
        }
    }
}
