using UnityEngine;

public class Aliasing : MonoBehaviour
{
	public bool UseLerp = true;
	private Vector3 currentState;
	private Vector3 previousState;

	[Header("FPS Benchmarks")]
	public int UpdateFPS;
	public int FixedUpdateFPS;
	private Benchmark updateBench = new Benchmark();
	private Benchmark fixedUpdateBench = new Benchmark();

	[Header("Lissajous curve")]
	public float ampX = 8;
	public float ampZ = 2;
	public int a = 1;
	public int b = 2;
	public float d = Mathf.PI / 2;
	public float step = 0.05f;
	public float angle = 0;

	void FixedUpdate()
	{
		previousState = currentState;
		angle += step;

		if (angle >= Mathf.PI * 2)
			angle = 0;

		currentState.x = ampX * Mathf.Sin(a * angle + d);
		currentState.z = ampZ * Mathf.Sin(b * angle);

		FixedUpdateFPS = fixedUpdateBench.Run();
	}

	void Update()
	{
		float alpha = (Time.time - Time.fixedTime) / Time.fixedDeltaTime;
		//Vector3 lerpState = currentState * alpha + previousState * (1.0f - alpha);
		Vector3 lerpState = Vector3.Lerp(previousState, currentState, alpha);

		if (UseLerp)
			transform.position = lerpState;
		else
			transform.position = currentState;

		UpdateFPS = updateBench.Run();
	}
}
