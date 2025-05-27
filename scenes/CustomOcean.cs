using Godot;
using System;

public partial class CustomOcean : MeshInstance3D
{
	ShaderMaterial material;
	Image noise;

	float noiseScale;
	float waveSpeed;
	float heightScale;

	float time;

	public override void _Ready()
	{
		material = Mesh.SurfaceGetMaterial(0) as ShaderMaterial;
		NoiseTexture2D waveTexture = material.GetShaderParameter("wave").As<NoiseTexture2D>();
		noise = waveTexture.Noise.GetSeamlessImage(512, 512);

		noiseScale = material.GetShaderParameter("noise_scale").AsSingle();
		waveSpeed = material.GetShaderParameter("wave_speed").AsSingle();
		heightScale = material.GetShaderParameter("height_scale").AsSingle();
	}

	public override void _Process(double delta)
	{
		time += (float)delta;
		material.SetShaderParameter("wave_time", time);
	}

	float getHeight(Vector3 worldPosition)
	{
		float uv_x = Mathf.Wrap(worldPosition.X / noiseScale + time * waveSpeed, 0, 1);
		float uv_y = Mathf.Wrap(worldPosition.Z / noiseScale + time * waveSpeed, 0, 1);

		Vector2 pixelPos = new Vector2(uv_x * noise.GetWidth(), uv_y * noise.GetHeight());
		return noise.GetPixelv(new Vector2I((int)pixelPos.X, (int)pixelPos.Y)).R * heightScale;
	}
}
