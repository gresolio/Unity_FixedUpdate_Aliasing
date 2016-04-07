using UnityEngine;

/// <summary>
/// http://gafferongames.com/game-physics/fix-your-timestep/
/// </summary>
public class Timestep : MonoBehaviour
{
	public struct State
	{
		public float x;
		public float v;

		public State(float x, float v)
		{
			this.x = x;
			this.v = v;
		}
	};

	struct Derivative
	{
		public float dx;
		public float dv;
	};

	State interpolate(State previous, State current, float alpha)
	{
		State state;
		state.x = current.x * alpha + previous.x * (1 - alpha);
		state.v = current.v * alpha + previous.v * (1 - alpha);
		return state;
	}

	float acceleration(State state, float t)
	{
		const float k = 10;
		const float b = 1;
		return -k * state.x - b * state.v;
	}

	Derivative evaluate(State initial, float t)
	{
		Derivative output;
		output.dx = initial.v;
		output.dv = acceleration(initial, t);
		return output;
	}

	Derivative evaluate(State initial, float t, float dt, Derivative d)
	{
		State state;
		state.x = initial.x + d.dx * dt;
		state.v = initial.v + d.dv * dt;
		Derivative output;
		output.dx = state.v;
		output.dv = acceleration(state, t + dt);
		return output;
	}

	void integrate(ref State state, float t, float dt)
	{
		Derivative a = evaluate(state, t);
		Derivative b = evaluate(state, t, dt * 0.5f, a);
		Derivative c = evaluate(state, t, dt * 0.5f, b);
		Derivative d = evaluate(state, t, dt, c);

		float dxdt = 1.0f / 6.0f * (a.dx + 2.0f * (b.dx + c.dx) + d.dx);
		float dvdt = 1.0f / 6.0f * (a.dv + 2.0f * (b.dv + c.dv) + d.dv);

		state.x = state.x + dxdt * dt;
		state.v = state.v + dvdt * dt;
	}

	[Header("FPS Benchmarks")]
	public int UpdateFPS;
	public int FixedUpdateFPS;
	public int TestCycleFPS;
	private Benchmark updateBench = new Benchmark();
	private Benchmark fixedUpdateBench = new Benchmark();
	private Benchmark testCycleBench = new Benchmark();

	[Header("Fix Your Timestep!")]
	public float initX = 40f;
	public float initV = 0;
	float currentTime = 0.0f;
	float accumulator = 0.0f;
	public float dt = 0.1f;
	public float t = 0.0f;
	private float alpha;
	State lerpState;
	State current;
	State previous;

	[Header("Render")]
	public GameObject gCurrent;
	public GameObject gPrevious;
	public GameObject gLerp;

	void Start()
	{
		current = new State(initX, initV);
		previous = current;

		Time.fixedDeltaTime = dt;
	}

	void FixedUpdate()
	{
		previous = current;
		integrate(ref current, t, dt);
		t += dt;

		FixedUpdateFPS = fixedUpdateBench.Run();
	}

	void Update()
	{
		/*
		float newTime = Time.realtimeSinceStartup;
		float deltaTime = newTime - currentTime;
		currentTime = newTime;

		if (deltaTime > 0.25f)
			deltaTime = 0.25f;

		accumulator += deltaTime;

		if (dt < 0.005f) // 200 FPS limit
			dt = 0.005f; // to prevent endless while loop

		while (accumulator >= dt) {
			accumulator -= dt;
			previous = current;
			integrate(ref current, t, dt);
			t += dt;

			TestCycleFPS = testCycleBench.Run();
		}

		alpha = accumulator / dt;
		*/

		alpha = (Time.time - Time.fixedTime) / Time.fixedDeltaTime;

		lerpState = interpolate(previous, current, alpha);

		gLerp.transform.position = new Vector3(lerpState.x, 0, 0);
		gCurrent.transform.position = new Vector3(current.x, 0, 0);
		gPrevious.transform.position = new Vector3(previous.x, 0, 0);

		if (Input.GetKeyDown(KeyCode.T)) { // Toggle current & previous
			gCurrent.SetActive(!gCurrent.activeInHierarchy);
			gPrevious.SetActive(!gPrevious.activeInHierarchy);
		}

		if (Input.GetKeyDown(KeyCode.R)) { // Reset our simulation
			current.x = previous.x = initX;
			current.v = previous.v = initV;
		}

		UpdateFPS = updateBench.Run();
	}

	void OnGUI()
	{
		GUI.color = Color.green;
		GUI.Label(new Rect(10, 10, 400, 20), "Update FPS: " + UpdateFPS);
		GUI.Label(new Rect(10, 30, 400, 20), "FixedUpdate FPS: " + FixedUpdateFPS);
		GUI.Label(new Rect(10, 50, 400, 20), "Keys: R - reset simulation, T - toggle current & previous visibility");
	}

}
