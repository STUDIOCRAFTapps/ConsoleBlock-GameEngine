using UnityEngine;
using System.Collections;

public class StoneTextureCreatorTool : MonoBehaviour {

	[Range(0f, 1f)]
	public float ApplyIronColor;
	[Range(0f, 1f)]
	public float ApplyGazBubbleMesh;
	[Range(0f, 1f)]
	public float CruchedLevelMesh;
	[Range(0f, 1f)]
	public float BrokenLevelMesh;
	[Range(0f, 1f)]
	public float ApplyVariantColorEditor;
	[Range(0f, 1f)]
	public float GraniteCompositionColor;
	[Range(0f, 1f)]
	public float DioriteCompositionColor;
	[Range(0f, 1f)]
	public float AndesiteCompositionColor;
	[Range(0f, 1f)]
	public float Darkness;
	[Range(0f, 1f)]
	public float ApplyLayeredMesh;

	public Color GraniteColor;
	public Color DioriteColor;
	public Color AndesiteColor;
	public Color IronVariation;
	public Color Normal;
	public Color FullDarkPoint;

	public Texture2D GazBubble;
	public Texture2D LayeredRock;
	public Texture2D Crunched;

	public MeshRenderer mr;
	public Material MainMaterial;
	Texture2D RockTexture;

	// Use this for initialization
	void Start () {
		mr = mr.GetComponent<MeshRenderer>();
		UpdateTexture();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Return)) {
			UpdateTexture();
		}
	}

	void UpdateTexture () {
		RockTexture = new Texture2D(16, 16);
		mr.material.mainTexture = RockTexture;
		mr.material.shader = MainMaterial.shader;
		RockTexture.filterMode = FilterMode.Point;
		
		for(int x = 0; x < 16; x++) {
			for(int y = 0; y < 16; y++) {
				float r = (ApplyIronColor * IronVariation.r + GraniteCompositionColor * GraniteColor.r + DioriteCompositionColor * DioriteColor.r + AndesiteCompositionColor * AndesiteColor.r) / 4;
				float g = (ApplyIronColor * IronVariation.g + GraniteCompositionColor * GraniteColor.g + DioriteCompositionColor * DioriteColor.g + AndesiteCompositionColor * AndesiteColor.g) / 4;
				float b = (ApplyIronColor * IronVariation.b + GraniteCompositionColor * GraniteColor.b + DioriteCompositionColor * DioriteColor.b + AndesiteCompositionColor * AndesiteColor.b) / 4;

				float BrokenIntesity = Random.Range(-BrokenLevelMesh, BrokenLevelMesh);
				r += (Random.Range(-ApplyVariantColorEditor, ApplyVariantColorEditor) + BrokenIntesity) / 2;
				g += (Random.Range(-ApplyVariantColorEditor, ApplyVariantColorEditor) + BrokenIntesity) / 2;
				b += (Random.Range(-ApplyVariantColorEditor, ApplyVariantColorEditor) + BrokenIntesity) / 2;

				r += (LayeredRock.GetPixel(x, y).grayscale - 1) * ApplyLayeredMesh;
				g += (LayeredRock.GetPixel(x, y).grayscale - 1) * ApplyLayeredMesh;
				b += (LayeredRock.GetPixel(x, y).grayscale - 1) * ApplyLayeredMesh;

				r += (Crunched.GetPixel(x, y).grayscale - 1) * CruchedLevelMesh;
				g += (Crunched.GetPixel(x, y).grayscale - 1) * CruchedLevelMesh;
				b += (Crunched.GetPixel(x, y).grayscale - 1) * CruchedLevelMesh;

				r += (GazBubble.GetPixel(x, y).grayscale - 1) * ApplyGazBubbleMesh;
				g += (GazBubble.GetPixel(x, y).grayscale - 1) * ApplyGazBubbleMesh;
				b += (GazBubble.GetPixel(x, y).grayscale - 1) * ApplyGazBubbleMesh;

				r -= Darkness;
				g -= Darkness;
				b -= Darkness;

				Color CColor = new Color(r, g, b);
				RockTexture.SetPixel(x, y, CColor);
			}
		}
		RockTexture.Apply();
	}
}
