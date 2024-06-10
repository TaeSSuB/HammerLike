using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueParser : MonoBehaviour
{
    public Dialogue[] Parse(string csvFileName)
    {
        List<Dialogue> dialogueList = new List<Dialogue>();
        TextAsset csvData = Resources.Load<TextAsset>(csvFileName);

        string[] data = csvData.text.Split(new char[]{'\n'});
        for(int i=1; i<data.Length;)
        {
            string[] row = data[i].Split(new char[] { ',' });

            Dialogue dialouge = new Dialogue();

            dialouge.name = row[1];
            Debug.Log(row[1]);
            List<string> contextList = new List<string>();
            do
            {
                contextList.Add(row[2]);
                Debug.Log(row[2]);
                if (++i < data.Length)
                {
                    row = data[i].Split(new char[] {','});
                }else
                {
                    break;
                }

            } while (row[0].ToString() == "");

            dialouge.contexts = contextList.ToArray();

            dialogueList.Add(dialouge);

        }
        return dialogueList.ToArray();
    }

   
}
