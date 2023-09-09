using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 싱글톤 클래스를 간편하게 사용하도록 도와주는 클래스, GlobalFunctionBase(MonoBehaviour) 를 상속받습니다.
/// </summary>
/// <typeparam name="T">제네릭 클래스(T)에 싱글톤으로 만들 클래스를 넣습니다.<para></para>
/// </typeparam>
public class GlobalSingleton<T> : GlobalFunctionBase where T : GlobalSingleton<T>
{
    // private -> T 로 받을 클래스의 _instance 는 외부에서 수정이 불가능하다.
    // static -> _instance 는 어디에서든 접근 가능한 전역 클래스이다. 
    // default -> _instance 를 초기화한다.
    private static T _instance = default;

    // public -> _instance 의 프로퍼티 (Instance) 를 접근 가능하게 열어준다.
    public static T Instance 
    {
        get
        {
            // { 조건문 : 만약 싱글톤으로 사용할 T 클래스의 _instance 가 default 거나 null 일 경우,
            if (GlobalSingleton<T>._instance == default || GlobalSingleton<T>._instance == null)
            {
                // 새로운 게임 오브젝트에 T 클래스를 컴포넌트로 붙여서 생성한다.
                GlobalSingleton<T>._instance = new GameObject().AddComponent<T>();
                // 이 싱글톤 오브젝트가 씬이 바뀌어도 파괴되지 않도록 한다.
                DontDestroyOnLoad(_instance.gameObject);
            }
            // } 조건문 종료

            // _instance 의 프로퍼티 (Instance) 에 접근하면 _instance 를 반환한다.
            return _instance;
        }
    }
}
