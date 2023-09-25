using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class CNPC : CUnitBase
{
    public Image interActionImage;
    public TextMeshProUGUI scriptCaption;
    public string[] scripts;
    public bool isPlayerEnter = false;

    public CPlayer player;
    public int scriptidx = 0;

    public virtual void Chat(string script)
    {
        scriptCaption.text = script;
        scriptidx++;
    }

    public virtual void Exitplayer()
    {
        Chat("");
        isPlayerEnter = false;
        player.isInterAction = false;
        interActionImage.enabled = false;
        player = null;
    }

    public virtual void ChatEnd()
    {
        Debug.Log(name +  "'s chat end");
    }

    // Start is called before the first frame update
    void Start()
    {
        interActionImage.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(isPlayerEnter)
        {
            if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(1))
            {
                if (scripts.Length > scriptidx)
                {
                    if (!player.isInterAction)
                    {
                        player.isInterAction = true;
                    }
                    Chat(scripts[scriptidx]);
                }
                else
                {
                    player.isInterAction = false;
                    ChatEnd();
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D col2d)
    {
        if(col2d.CompareTag("Player") && player == null)
        {
            scriptidx = 0;
            isPlayerEnter = true;
            interActionImage.enabled = true;
            player = col2d.GetComponent<CPlayer>();
        }
    }

    private void OnTriggerExit2D(Collider2D col2d)
    {
        if (col2d.CompareTag("Player") && player != null)
        {
            Exitplayer();
        }
    }
}
