using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SpeechBox : MonoBehaviour
{
    [SerializeField] TMP_Text message;
    [SerializeField] Animator animator;

    Button button;


    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        button.onClick.AddListener(() =>
        {
            animator.enabled = true;
            AudioManager.Instance.PlayVoice(message.text, () =>
            {
                animator.Rebind();
                animator.Update(0f);
                animator.enabled = false;
            });
        });
    }

    private void OnDisable()
    {
        button.onClick.RemoveAllListeners();

        animator.Rebind();
        animator.Update(0f);
        animator.enabled = false;
    }
}
