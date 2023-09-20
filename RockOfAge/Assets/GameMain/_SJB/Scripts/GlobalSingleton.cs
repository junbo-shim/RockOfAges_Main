using UnityEngine;

/// <summary>
/// 클래스를 싱글톤으로 쉽게 변환해주는 클래스입니다, GlobalFunctionBase(MonoBehaviour) 상속받고 있습니다.
/// <para></para>
/// 사용법 : 싱글톤으로 만들 클래스에 이 클래스를 상속시키면 됩니다.
/// <para></para>
/// 예시) public class ItemManager : GlobalSingleton(ItemManager)
/// </summary>
/// <typeparam name="T">제네릭 클래스(T)에 싱글톤으로 만들 클래스만 넣을 수 있습니다.<para></para>
/// </typeparam>
public class GlobalSingleton<T> : GlobalFunctionBase where T : GlobalSingleton<T>
{
    // private -> T 에 들어가는 클래스는 _instance 라는 필드를 가지며 접근 보호 수준이 private 입니다.
    // static -> _instance 를 static 으로 만들어 프로그램 종료까지 메모리에 남아있습니다. 
    // default -> _instance 를 초기화합니다.
    private static T _instance = default;

    // public -> _instance 의 프로퍼티 (Instance) 를 생성하여 외부에서 접근할 수 있도록 합니다.
    public static T Instance
    {
        get
        {
            // { 조건문 : 싱글톤으로 만들 T 의 _instance 가 default 이거나 null 상태라면,
            if (GlobalSingleton<T>._instance == default || GlobalSingleton<T>._instance == null)
            {
                // T 를 게임 오브젝트에 컴포넌트로 붙여서 인스턴스화 합니다.
                GlobalSingleton<T>._instance = new GameObject().AddComponent<T>();
                _instance.GetComponent<T>().enabled = true;

                // 싱글톤으로 컴포넌트화 한 게임 오브젝트는 Scene 이 바뀌어도 파괴되지 않습니다.
                DontDestroyOnLoad(_instance.gameObject);
            }
            // } 조건문 종료

            // _instance 의 프로퍼티 (Instance) 에 _instance 를 넣고 반환합니다.
            return _instance;
        }
    }

    public void CreateThisManager() 
    {
        /*Intentionally Emptied*/
    }
}
