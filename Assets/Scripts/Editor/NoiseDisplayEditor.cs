using System.Linq;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

[CustomEditor(typeof(NoiseDisplay))]
public class NoiseDisplayEditor : Editor
{
	private NoiseDisplay m_noiseDisplay;
	private string[] m_noiseModelPresets;
	private int m_noiseModelPresetsIndex;

	AnimBool m_groupInformation;

	void OnEnable()
	{
		this.m_noiseModelPresets = Noise.Presets.PresetNoises.Keys.ToArray();
		m_noiseDisplay = serializedObject.targetObject as NoiseDisplay;
		if (m_noiseDisplay != null) m_noiseModelPresetsIndex = m_noiseDisplay.NoisePresetIndex;

		m_groupInformation = new AnimBool(true);
		m_groupInformation.valueChanged.AddListener(Repaint);
	}

	public override void OnInspectorGUI()
	{
		if (m_noiseDisplay == null) return;

		// Make NoiseModel Choosable
		m_noiseDisplay.NoisePresetIndex = EditorGUILayout.Popup("Noise Model", m_noiseModelPresetsIndex,
			m_noiseModelPresets);
		if (m_noiseDisplay.NoisePresetIndex != m_noiseModelPresetsIndex)
		{
			m_noiseModelPresetsIndex = m_noiseDisplay.NoisePresetIndex;
		}

		//Draw default Inspector
		DrawDefaultInspector();
		
		//If toggled make wavelength a power of two and adjust frequency to it
		if (m_noiseDisplay.usePowerOfTwoWaveLength)
		{
			m_noiseDisplay.waveLength = Mathf.ClosestPowerOfTwo((int) (2f / m_noiseDisplay.frequency)) * 0.5f;
			m_noiseDisplay.frequency = (1f / m_noiseDisplay.waveLength);
		} else
		{
			m_noiseDisplay.waveLength = 1f / m_noiseDisplay.frequency;
		}

		m_groupInformation.target = EditorGUILayout.Foldout(m_groupInformation.target, "Derived Information", true);
		if (EditorGUILayout.BeginFadeGroup(m_groupInformation.faded))
		{
			EditorGUI.indentLevel++;
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel("Wave Length");
			EditorGUILayout.FloatField(m_noiseDisplay.waveLength);
			EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Computation Time (in ms)");
            EditorGUILayout.LongField(m_noiseDisplay.computationTime);
            EditorGUILayout.EndHorizontal();

			EditorGUI.indentLevel--;
		}

		EditorGUILayout.EndFadeGroup();

		if (GUILayout.Button("Generate")) m_noiseDisplay.GenNoise();
		if (GUILayout.Button("Save to File")) m_noiseDisplay.SaveToFile();
	}
}