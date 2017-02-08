using UnityEngine;

/// <summary>
/// This script is used to switch between editing modes.
/// </summary>

public class ExampleEditModeGUI : MonoBehaviour
{
	public GameObject[] choices;

	void OnGUI ()
	{
		GUILayout.BeginVertical("box");
		GUILayout.BeginHorizontal();

		GameObject current = null;
		GameObject selected = null;

		for (int i = 0; i < choices.Length; ++i)
		{
			GameObject go = choices[i];
			bool highlight = go.activeInHierarchy;
			if (highlight) current = go;

			GUI.backgroundColor = highlight ? Color.green : Color.gray;

			if (GUILayout.Toggle(highlight, "<b>" + go.name + "</b>", "Button") && !highlight)
				selected = go;

			GUI.backgroundColor = Color.white;
		}

		GUILayout.EndHorizontal();
		GUILayout.EndVertical();

		// Prevent mouse input
		Rect rect = GUILayoutUtility.GetLastRect();
		Vector2 pos = Input.mousePosition;
		pos.y = Screen.height - pos.y;
		if (rect.Contains(pos))
			ExampleCamera.allowMouseInput = false;

		if (current != selected && selected != null)
		{
			current.SetActive(false);
			selected.SetActive(true);
		}
	}
}
