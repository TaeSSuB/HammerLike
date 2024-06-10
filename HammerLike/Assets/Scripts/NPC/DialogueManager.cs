using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;

    [SerializeField] string csvFileName;

    Dictionary<int, Dialogue> dialogueDic = new Dictionary<int, Dialogue>();

    public static bool isFinish = false;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DialogueParser thePaser = GetComponent<DialogueParser>();
            Dialogue[] dialogues = thePaser.Parse(csvFileName);
            for(int i =0; i< dialogues.Length; i++)
            {
                dialogueDic.Add(i + 1, dialogues[i]);
            }
            isFinish = true;
        }
    }

    public Dialogue[] GetDialogue(int startNum, int endNum)
    {
        List<Dialogue> dialogueList = new List<Dialogue>();

        for(int i=0; i<=endNum-startNum; i++)
        {                                          
            dialogueList.Add(dialogueDic[startNum+i]);
        }

        return dialogueList.ToArray();
    }
}
