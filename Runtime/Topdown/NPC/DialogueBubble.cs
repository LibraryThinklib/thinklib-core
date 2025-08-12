using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[AddComponentMenu("Thinklib/Topdown/NPC/Dialogue Bubble", -100)]
public class DialogueBubble : MonoBehaviour
{
    [Header("Campo de texto")]
    public Text textField;

    private Coroutine typeCoroutine;

    /// <summary>
    /// Indica se o texto ainda est� sendo exibido letra por letra.
    /// </summary>
    public bool IsTyping { get; private set; } = false;

    /// <summary>
    /// Define o texto a ser exibido no bal�o, com ou sem efeito de digita��o.
    /// </summary>
    /// <param name="text">Texto completo da fala.</param>
    /// <param name="useTypewriter">Se verdadeiro, ativa o efeito de digita��o letra por letra.</param>
    /// <param name="speed">Velocidade da digita��o (tempo entre letras).</param>
    public void SetText(string text, bool useTypewriter = false, float speed = 0.05f)
    {
        if (textField == null)
        {
            Debug.LogWarning("Campo 'textField' do bal�o de fala n�o est� atribu�do!");
            return;
        }

        if (typeCoroutine != null)
        {
            StopCoroutine(typeCoroutine);
        }

        if (useTypewriter)
        {
            typeCoroutine = StartCoroutine(TypeText(text, speed));
        }
        else
        {
            textField.text = text;
            IsTyping = false;
        }
    }

    private IEnumerator TypeText(string text, float speed)
    {
        IsTyping = true;
        textField.text = "";
        foreach (char c in text)
        {
            textField.text += c;
            yield return new WaitForSeconds(speed);
        }
        IsTyping = false;
    }
}
