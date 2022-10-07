using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    Dictionary<string, TMP_Text> textDictionary = new();
    Dictionary<string, Button> buttonDictionary = new();
    [SerializeField] TMP_InputField pseudoField;

    void Awake()
    {
    }
    // Start is called before the first frame update
    void Start()
    {
        foreach (var text in GameObject.FindGameObjectsWithTag("TextMeshPro"))
            textDictionary.Add(text.name, text.GetComponent<TMP_Text>());

        foreach (var button in GameObject.FindGameObjectsWithTag("Button"))
            buttonDictionary.Add(button.name, button.GetComponent<Button>());

        if (pseudoField != null)
            pseudoField.onSubmit.AddListener(UpdatePseudo);

        if(buttonDictionary.Count > 0)
            AddAllButtonOnClickListeners();

        GameManager.instance.LoadSerializableData();

        RefreshAllTextsUi();
    }
    void AddAllButtonOnClickListeners()
    {
        AddButtonOnClickListener("StartButton", GameManager.instance.StartGame);
        AddButtonOnClickListener("ValidButton", UpdatePseudo);
        AddButtonOnClickListener("RestartButton", GameManager.instance.BackToStartMenu);
        AddButtonOnClickListener("QuitButton", GameManager.instance.QuitApplication);
    }
    void AddButtonOnClickListener(string buttonName, UnityAction callback)
    {
        if (buttonDictionary.TryGetValue(buttonName, out var button))
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(callback);
        }
    }
    public void RefreshAllTextsUi()
    {
        string textToInject;

        textToInject = GameManager.instance.bestPlayerPseudo == null || GameManager.instance.bestPlayerPseudo == "" ? "None" : GameManager.instance.bestPlayerPseudo;
        RefreshTextUI("BestPseudoOnlyTMP", "" + textToInject);
        textToInject = GameManager.instance.bestScore <= 0 ? "No Score" : GameManager.instance.bestScore.ToString();
        RefreshTextUI("BestScoreOnlyTMP", textToInject);

        textToInject = GameManager.instance.playerPseudo == null || GameManager.instance.playerPseudo == "" ? "None" : GameManager.instance.playerPseudo;
        RefreshTextUI("PseudoOnlyTMP", textToInject);
        
        textToInject = GameManager.instance.playerScore <= 0 ? "No Score" : GameManager.instance.playerScore.ToString();
        RefreshTextUI("ScoreOnlyTMP", textToInject);
    }
    public void RefreshTextUI(string nameTMP, string textToInject)
    {
        if(textDictionary.TryGetValue(nameTMP, out TMP_Text text))
        {
            text.text = textToInject;
            if (text.name == "ValidTMP")
            {
                if(buttonDictionary.TryGetValue("ValidButton", out var validButton))
                {
                    if (text.text == "Validated !")
                        validButton.GetComponent<Image>().color = new Color(.07f, .28f, .13f);
                    if (text.text == "Validate")
                        validButton.GetComponent<Image>().color = new Color(0.16f, 0.84f, 0.36f);
                }
            }
            else if(text.name == "PseudoOnlyTMP")
            {
                if (textDictionary.TryGetValue("ChooseTMP", out TMP_Text chooseText))
                {
                    if (text.text == "" || text.text == "None" || text.text == null)
                    {
                        text.text = "";
                        chooseText.gameObject.SetActive(true);
                    }
                    else
                    {
                        chooseText.gameObject.SetActive(false);
                    }
                }
            }
        }
    }
    public void UpdatePseudo()
    {
        UpdatePseudo(pseudoField.text);
    }
    private void UpdatePseudo(string playerNameText)
    {
        GameManager.instance.playerPseudo = playerNameText;
        RefreshTextUI("PseudoOnlyTMP", GameManager.instance.playerPseudo);

        RefreshTextUI("ValidTMP", "Validated !");

        StartCoroutine("RestoreValidateButton");
    }

    IEnumerator RestoreValidateButton()
    {
        yield return new WaitForSeconds(2);

        RefreshTextUI("ValidTMP", "Validate");
    }
}
