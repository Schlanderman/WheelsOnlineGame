using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DebugChangeCameraPosition : MonoBehaviour
{
    [SerializeField] private Button camChangeLeft;
    [SerializeField] private Button camChangeRight;

    [SerializeField] private Camera viewCamera;
    [SerializeField] private Transform positionPlayerOne;
    [SerializeField] private Transform positionPlayerTwo;

    //HUD-Buttons zum Aktivieren/Deaktivieren
    [SerializeField] private GameObject playerOneHud;
    [SerializeField] private GameObject playerTwoHud;

    //HP-Anzeigen zum Aktivieren/Deaktivieren
    [SerializeField] private GameObject hpScreensPlayerOne;
    [SerializeField] private GameObject hpScreensPlayerTwo;

    private bool isOnPlayerOnePosition = true;  //Startet auf der Position von Spieler 1

    private float transitionDuration = 0.3f;    //Dauer der Bewegung in Sekunden
    private bool isTransitioning = false;       //Verhindert eingabe während der Bewegung

    private void Awake()
    {
        camChangeLeft.onClick.AddListener(() => ChangeCameraPositionLeft());
        camChangeRight.onClick.AddListener(() => ChangeCameraPositionRight());
    }

    private void Start()
    {
        // Kamera initial auf die Position von Spieler 1 setzen
        MoveCameraToPosition(positionPlayerOne);

        // Buttons initial aktivieren/deaktivieren
        UpdateButtonStates();
    }

    private void ChangeCameraPositionLeft()
    {
        if (isOnPlayerOnePosition && !isTransitioning)
        {
            MoveCameraToPosition(positionPlayerTwo);
            isOnPlayerOnePosition = false;
            UpdateButtonStates();
        }
    }

    private void ChangeCameraPositionRight()
    {
        if (!isOnPlayerOnePosition && !isTransitioning)
        {
            MoveCameraToPosition(positionPlayerOne);
            isOnPlayerOnePosition = true;
            UpdateButtonStates();
        }
    }

    private void MoveCameraToPosition(Transform targetPosition)
    {
        StartCoroutine(MoveCameraSmoothly(targetPosition));
    }

    private void UpdateButtonStates()
    {
        camChangeLeft.interactable = isOnPlayerOnePosition;    //Deaktiviert, wenn auf Position von Spieler 2
        playerOneHud.SetActive(isOnPlayerOnePosition);
        hpScreensPlayerOne.SetActive(isOnPlayerOnePosition);

        camChangeRight.interactable = !isOnPlayerOnePosition;    //Deaktiviert, wenn auf Position von Spieler 1
        playerTwoHud.SetActive(!isOnPlayerOnePosition);
        hpScreensPlayerTwo.SetActive(!isOnPlayerOnePosition);
    }

    private IEnumerator MoveCameraSmoothly(Transform targetPosition)
    {
        isTransitioning = true;

        Vector3 startPosition = viewCamera.transform.position;
        Quaternion startRotation = viewCamera.transform.rotation;

        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            float t = elapsedTime / transitionDuration;

            // Apply Easing (Smoothstep for smooth start and stop)
            t = t * t * (3f - 2f * t);

            //Lerp Position and Rotation
            viewCamera.transform.position = Vector3.Lerp(startPosition, targetPosition.position, t);
            viewCamera.transform.rotation = Quaternion.Lerp(startRotation, targetPosition.rotation, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        viewCamera.transform.position = targetPosition.position;
        viewCamera.transform.rotation = targetPosition.rotation;

        isTransitioning = false;
    }
}
