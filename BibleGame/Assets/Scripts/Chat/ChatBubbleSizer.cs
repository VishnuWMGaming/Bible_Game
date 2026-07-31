using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class ChatBubbleSizer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private LayoutElement textLayoutElement;
    [SerializeField] private float minTextWidth = 40f;
    [SerializeField] private float maxTextWidth = 220f;

    private string lastText = null;

    void LateUpdate()
    {
        //if (messageText.text != lastText)
        //{
        //    lastText = messageText.text;
        //    ResizeBubble();
        //}
    }

    public void ResizeBubble()
    {
        messageText.ForceMeshUpdate(true, true);

        Vector2 unconstrained = messageText.GetPreferredValues(messageText.text);

        float finalWidth;

        if (unconstrained.x > maxTextWidth)
        {
            finalWidth = maxTextWidth;
            messageText.enableWordWrapping = true;
        }
        else
        {
            finalWidth = unconstrained.x;
            messageText.enableWordWrapping = false;
        }

        textLayoutElement.preferredWidth = finalWidth;

        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());

        if (transform.parent != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent.GetComponent<RectTransform>());
        }
    }
}