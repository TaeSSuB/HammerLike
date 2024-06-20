using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // 씬 관리를 위해 추가

public class ChangeCursor : MonoBehaviour
{
    [System.Serializable]
    public struct CursorTexture
    {
        public int index;
        public Texture2D texture;
    }

    public static ChangeCursor Instance { get; private set; }
    [SerializeField]
    private CursorTexture[] cursorTextures;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded; // 씬이 로드될 때마다 호출할 이벤트 리스너 등록
        }
        
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬 이름에 따라 초기 커서 설정
        if (scene.name == "Mainmenu")
        {
            SetCursorWhenReady(3); // MainMenu 씬에서는 3번 인덱스 커서 사용
        }
        else
        {
            SetCursorWhenReady(0); // 그 외 씬에서는 기본적으로 0번 인덱스 커서 사용
        }
    }

    public void SetCursorAttack()
    {
        StartCoroutine(SetCursorByAttack());
    }

    public IEnumerator SetCursorByAttack()
    {
        SetCursorWhenReady(1); // 피격 시 1번 인덱스 커서로 변경
        yield return new WaitForSeconds(0.2f); // 0.2초 대기
        SetCursorWhenReady(0); // 원래 커서로 복귀
    }

    public void OnButtonClick()
    {
        StartCoroutine(HandleButtonClick());
    }

    private IEnumerator HandleButtonClick()
    {
        SetCursorWhenReady(2); // 버튼 클릭 시 2번 인덱스 커서로 변경
        yield return new WaitForSeconds(0.5f); // 0.5초 대기
        SetCursorWhenReady(3); // 다시 3번 인덱스 커서로 변경
    }

    // 주어진 인덱스에 해당하는 텍스처로 커서 이미지를 변경
    public void SetCursorByIndex(int index)
    {
        foreach (var cursorTexture in cursorTextures)
        {
            if (cursorTexture.index == index)
            {
                Cursor.SetCursor(cursorTexture.texture, Vector2.zero, CursorMode.Auto);
                break;
            }
        }
    }

    // 텍스처 로딩이 준비되었는지 확인 후 커서를 설정
    public void SetCursorWhenReady(int index)
    {
        StartCoroutine(WaitForTextureAndSetCursor(index));
    }

    private IEnumerator WaitForTextureAndSetCursor(int index)
    {
        yield return new WaitUntil(() => cursorTextures[index].texture.isReadable); // 텍스처가 읽기 가능할 때까지 대기
        SetCursorByIndex(index); // 준비된 텍스처로 커서 설정
    }
}
