using UnityEngine;

public class Benchmark
{
	public int CurrentFPS
	{
		get { return m_CurrentFps; }
	}

	private const float c_FpsMeasurePeriod = 0.5f;
	private bool m_FirstUpdate = true;
	private float m_FpsNextPeriod = 0;
	private int m_FpsAccumulator = 0;
	private int m_CurrentFps;

	public int Run()
	{
		if (m_FirstUpdate) {
			m_FirstUpdate = false;
			m_FpsNextPeriod = Time.realtimeSinceStartup + c_FpsMeasurePeriod;
		}

		m_FpsAccumulator++;

		if (Time.realtimeSinceStartup > m_FpsNextPeriod) {
			m_CurrentFps = (int)(m_FpsAccumulator / c_FpsMeasurePeriod);
			m_FpsAccumulator = 0;
			m_FpsNextPeriod += c_FpsMeasurePeriod;
		}

		return m_CurrentFps;
	}

}
