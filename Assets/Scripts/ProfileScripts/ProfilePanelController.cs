using UnityEngine;

public class ProfilePanelController : MonoBehaviour
{
    [SerializeField] private GameObject savetyButton;
    private Animator animator;
    private bool isExpanded = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        savetyButton.SetActive(false);
    }

    public void TogglePanel()
    {
        isExpanded = !isExpanded;
        savetyButton.SetActive(isExpanded);
        animator.SetBool("isExpanded", isExpanded);
        AudioManager.Instance.PlaySoundClip(SoundClipRef.Button, SoundSourceRef.UISource, 0.5f);
    }
}
