using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCursor : MonoBehaviour
{
    // 인덱스와 텍스처를 연결하는 구조체 정의
    [System.Serializable]
    public struct CursorTexture
    {
        public int index;
        public Texture2D texture;
    }
    public static ChangeCursor Instance { get; private set; }
    // 커서 텍스처 배열
    [SerializeField]
    private CursorTexture[] cursorTextures;

    // Start 메서드는 게임 시작 시 최초로 실행됨
    void Start()
    {
        // 첫 번째 커서 이미지를 설정 (만약 배열이 비어 있지 않은 경우)
        if (cursorTextures.Length > 0)
        {
            SetCursorByIndex(0);
        }
    }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetCursorAttack()
    {
        StartCoroutine(SetCursorByAttack());
    }

    public IEnumerator SetCursorByAttack()
    {
        SetCursorByIndex(1);
        yield return new WaitForSeconds(0.2f);

        SetCursorByIndex(0);
    }

        // Update 메서드는 매 프레임마다 실행됨
        /*void Update()
        {
            // 예시: 숫자 키(0-9)를 눌러 커서 이미지를 변경
            for (int i = 0; i < cursorTextures.Length; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha0 + i))
                {
                    SetCursorByIndex(i);
                }
            }
        }*/

        // 주어진 인덱스에 해당하는 텍스처로 커서 이미지를 변경
        public void SetCursorByIndex(int index)
    {
        // 해당 인덱스에 일치하는 커서 텍스처 찾기
        foreach (var cursorTexture in cursorTextures)
        {
            if (cursorTexture.index == index)
            {
                Cursor.SetCursor(cursorTexture.texture, Vector2.zero, CursorMode.Auto);
                break;
            }
        }
    }
}
