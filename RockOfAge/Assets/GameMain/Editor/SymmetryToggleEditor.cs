using UnityEditor;
using UnityEngine;

public class SymmetryToggleEditor : EditorWindow
{
    private bool isSymmetryEnabled = false;
    private bool symmetryX = true;
    private bool symmetryY = true;
    private bool symmetryZ = true;
    private bool rotateX = true;
    private bool rotateY = true;

    [MenuItem("Tools/Toggle Symmetry")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow<SymmetryToggleEditor>("Symmetry Toggle");
    }

    private void OnGUI()
    {
        GUILayout.Label("Symmetry Toggle", EditorStyles.boldLabel);

        isSymmetryEnabled = EditorGUILayout.Toggle("Enable Symmetry", isSymmetryEnabled);
        symmetryX = EditorGUILayout.Toggle("Symmetry X", symmetryX);
        symmetryY = EditorGUILayout.Toggle("Symmetry Y", symmetryY);
        symmetryZ = EditorGUILayout.Toggle("Symmetry Z", symmetryZ);
        rotateX = EditorGUILayout.Toggle("Rotate X", rotateX);
        rotateY = EditorGUILayout.Toggle("Rotate Y", rotateY);

        if (GUILayout.Button("Apply Symmetry"))
        {
            ApplySymmetry();
        }
    }

    private void ApplySymmetry()
    {
        if (Selection.gameObjects != null && Selection.gameObjects.Length > 0)
        {
            foreach (GameObject selectedObject in Selection.gameObjects)
            {
                Vector3 position = selectedObject.transform.position;
                Vector3 scale = selectedObject.transform.localScale;

                if (isSymmetryEnabled)
                {
                    // 대칭 위치 계산
                    Vector3 symmetryPosition = position;
                    if (symmetryX)
                    {
                        symmetryPosition.x *= -1; // X 축을 기준으로 대칭
                    }
                    if (symmetryY)
                    {
                        symmetryPosition.y *= -1; // Y 축을 기준으로 대칭
                    }
                    if (symmetryZ)
                    {
                        symmetryPosition.z *= -1; // Z 축을 기준으로 대칭
                    }

                    // 오브젝트 복제 및 대칭 위치에 배치
                    GameObject newObject = Instantiate(selectedObject, symmetryPosition, selectedObject.transform.rotation);
                    newObject.transform.localScale = scale;

                    // 로컬 회전값 반전
                    Vector3 localRotation = newObject.transform.localEulerAngles;
                    if (rotateX)
                    {
                        localRotation.x = (localRotation.x > 180) ? localRotation.x - 180 : localRotation.x + 180;
                    }
                    if (rotateY)
                    {
                        localRotation.y = (localRotation.y > 180) ? localRotation.y - 180 : localRotation.y + 180;
                    }
                    newObject.transform.localEulerAngles = localRotation;

                    // 새로 생성한 오브젝트를 선택
                    Selection.activeGameObject = newObject;
                }
            }
        }
    }
}