using System.IO;
using System.Linq;
using Noise;
using UnityEngine;

[ExecuteInEditMode]
public class NoiseDisplay : MonoBehaviour
{
	public GameObject cpuRenderTarget;
	public GameObject gpuRenderTarget;

	#region Basic Settings

	[Header("Basic Settings")]
	public RenderType renderType = RenderType.Render2D;
	[Range(1, 512)]
	public int resolution = 512;
	[Space(5)]

	#endregion

	#region Noise Settings

	[Header("Noise Settings")]
	public bool useWorldSpace;
	public bool usePowerOfTwoWaveLength;
	public AnimationCurve splerpCurve;

	#endregion

	#region Noise Parameters

	[Header("Noise Parameter")]
	[Range(0f, 2f)]
	[Tooltip(
		"Frequency between 0 and 2 in percentage. Frequency 1 means for every pixel of resolution there is one value. Values above 1 will result in Aliasing."
	)]
	public float frequency = 1f;
	[Range(0f, 1f)]
	[Tooltip("The peak Amplitude (aka Value/Grayscale Color)")]
	public float amplitude = 1f;

	#endregion

	#region CustomInspectorData

	#region Information Value

	[HideInInspector]
	public float waveLength = 1f;
	[HideInInspector]
	public long computationTime;

	#endregion

	#region Choosable NoiseModel Preset

	[SerializeField]
	[HideInInspector]
	private int m_noisePresetIndex;
	public int NoisePresetIndex
	{
		get { return m_noisePresetIndex; }
		set
		{
			m_noisePresetIndex = value;
			m_noiseStruct.noiseModel = Presets.PresetNoises[Presets.PresetNoises.Keys.ElementAt(m_noisePresetIndex)];
			if (m_gpuMeshRenderer != null)
			{
				m_gpuMeshRenderer.sharedMaterial.shader = m_noiseStruct.noiseModel.NoiseShader;
			}
		}
	}

	#endregion

	#endregion

	#region PrivateMembers 

	private NoiseStruct m_noiseStruct;

	private MeshRenderer m_cpuMeshRenderer;
	private Texture2D m_cpuNoiseTexture;
	private Texture2D CpuNoiseTexture
	{
		get
		{
			if (m_cpuNoiseTexture == null)
			{
				m_cpuNoiseTexture = new Texture2D(resolution, resolution, TextureFormat.RGBA32, false)
				{
					name = "NoiseTexture_CPU",
					filterMode = FilterMode.Point,
					wrapMode = TextureWrapMode.Clamp,
					anisoLevel = 0
				};
			}
			return m_cpuNoiseTexture;
		}
	}

	private MeshRenderer m_gpuMeshRenderer;
	private Texture2D m_splerpCurveTexture;
	private Texture2D SplerpCurveTexture
	{
		get
		{
			if (m_splerpCurveTexture == null)
			{
				m_splerpCurveTexture = new Texture2D(256, 1, TextureFormat.ARGB32, false, true)
				{
					name = "Lerp Curve Texture",
					wrapMode = TextureWrapMode.Clamp,
					filterMode = FilterMode.Bilinear,
					anisoLevel = 0,
					hideFlags = HideFlags.DontSave
				};
			}

			return m_splerpCurveTexture;
		}
	}

	#endregion

	private void OnValidate()
	{
		if (cpuRenderTarget != null) m_cpuMeshRenderer = cpuRenderTarget.GetComponent<MeshRenderer>();
		if (gpuRenderTarget != null) m_gpuMeshRenderer = gpuRenderTarget.GetComponent<MeshRenderer>();
		if (splerpCurve != null) GenSplerpCurveTexture();
	}

	void Awake()
	{
		NoisePresetIndex = m_noisePresetIndex;

		if (cpuRenderTarget == null) cpuRenderTarget = GameObject.FindGameObjectWithTag("Target_NoiseCPU");
		if (cpuRenderTarget != null) m_cpuMeshRenderer = cpuRenderTarget.GetComponent<MeshRenderer>();

		if (gpuRenderTarget == null) gpuRenderTarget = GameObject.FindGameObjectWithTag("Target_NoiseGPU");
		if (gpuRenderTarget != null) m_gpuMeshRenderer = gpuRenderTarget.GetComponent<MeshRenderer>();

		if (splerpCurve != null) GenSplerpCurveTexture();
		UpdateNoiseStruct();
	}

	void Update()
	{
		if (m_noiseStruct.noiseModel == null) return;
		UpdateNoiseStruct();

		if (m_cpuMeshRenderer) m_cpuMeshRenderer.sharedMaterial.mainTexture = CpuNoiseTexture;

		if (m_gpuMeshRenderer) m_noiseStruct.noiseModel.UpdateNoise(m_gpuMeshRenderer, m_noiseStruct);
		//TODO: Call only when needed
		MakeRenderPixelPerfect();
	}

	public void GenNoise()
	{
		CpuNoiseTexture.Resize(resolution, resolution);
		Generator.Generate(m_noiseStruct, ref m_cpuNoiseTexture, out computationTime);
	}

	public void GenSplerpCurveTexture()
	{
		var colors = new Color[256];
		for (float i = 0f; i <= 1f; i += 1f / 255f)
		{
			var t = Mathf.Clamp(splerpCurve.Evaluate(i), 0f, 1f);
			colors[(int) Mathf.Floor(i * 255f)] = new Color(t, t, t, 1f);
		}
		SplerpCurveTexture.SetPixels(colors);
		SplerpCurveTexture.Apply();
	}

	//TODO: This is annoying, make noiseStruct editable right away.
	public void UpdateNoiseStruct()
	{
		//m_noiseStruct.noiseModel = m_noiseStruct.noiseModel;
		m_noiseStruct.splerp = splerpCurve;
		m_noiseStruct.splerpTexture = SplerpCurveTexture;
		m_noiseStruct.renderType = renderType;
		m_noiseStruct.resolution = resolution;
		m_noiseStruct.frequency = frequency;
		m_noiseStruct.amplitude = amplitude;
		m_noiseStruct.worldTransform = useWorldSpace ? transform : null;
	}

	public void SaveToFile()
	{
		byte[] bytes = CpuNoiseTexture.EncodeToPNG();
		File.WriteAllBytes(Application.dataPath + "/../NoiseOutput.png", bytes);
	}

	private void MakeRenderPixelPerfect()
	{
		// Update renderTargetsScale to be pixelperfect for orthographic camera
		float scale = (Screen.height / 2.0f) / Camera.main.orthographicSize;
		var cpuTransformLocalScale = cpuRenderTarget.transform.localScale;
		cpuTransformLocalScale.x = CpuNoiseTexture.width / scale;
		cpuTransformLocalScale.y = CpuNoiseTexture.height / scale;
		cpuRenderTarget.transform.localScale = cpuTransformLocalScale;
		gpuRenderTarget.transform.localScale = cpuTransformLocalScale;
	}
}