using UnityEngine;

namespace HookGun;

public class ShopKiosk : MonoBehaviour, IInteractibleConstant, IInteractible
{
    private const float InteractTime = 0.4f;

    private static Material? _kioskMaterial;
    private static Material? _signMaterial;

    public bool holdOnFinish => false;

    public static GameObject Build(Vector3 position, Quaternion rotation)
    {
        var root = new GameObject("HookGun_ShopKiosk");
        root.transform.SetPositionAndRotation(position, rotation);

        var body = GameObject.CreatePrimitive(PrimitiveType.Cube);
        body.name = "Body";
        body.transform.SetParent(root.transform, worldPositionStays: false);
        body.transform.localPosition = new Vector3(0f, 0.6f, 0f);
        body.transform.localScale = new Vector3(1.0f, 1.2f, 1.0f);
        ApplyMaterial(body, GetKioskMaterial());

        var top = GameObject.CreatePrimitive(PrimitiveType.Cube);
        top.name = "Top";
        top.transform.SetParent(root.transform, worldPositionStays: false);
        top.transform.localPosition = new Vector3(0f, 1.32f, 0f);
        top.transform.localScale = new Vector3(1.15f, 0.12f, 1.15f);
        ApplyMaterial(top, GetSignMaterial());

        var sign = new GameObject("Sign");
        sign.transform.SetParent(root.transform, worldPositionStays: false);
        sign.transform.localPosition = new Vector3(0f, 1.95f, 0f);
        sign.transform.localScale = Vector3.one;

        var tm = sign.AddComponent<TextMesh>();
        tm.text = "SHOP";
        tm.fontSize = 64;
        tm.characterSize = 0.06f;
        tm.anchor = TextAnchor.MiddleCenter;
        tm.alignment = TextAlignment.Center;
        tm.color = Color.white;
        tm.fontStyle = FontStyle.Bold;
        var billboard = sign.AddComponent<Billboard>();
        billboard.mesh = sign;

        // Снести лишние коллайдеры с примитивов, оставить один на body
        foreach (var col in root.GetComponentsInChildren<Collider>())
        {
            Object.Destroy(col);
        }
        var bigCollider = root.AddComponent<BoxCollider>();
        bigCollider.center = new Vector3(0f, 0.7f, 0f);
        bigCollider.size = new Vector3(1.3f, 1.6f, 1.3f);

        root.AddComponent<ShopKiosk>();
        return root;
    }

    private static void ApplyMaterial(GameObject go, Material mat)
    {
        var renderer = go.GetComponent<Renderer>();
        if (renderer != null) renderer.sharedMaterial = mat;
    }

    private static Material GetKioskMaterial()
    {
        if (_kioskMaterial != null) return _kioskMaterial;
        var shader = FindUsableShader();
        var mat = new Material(shader)
        {
            name = "HookGun_KioskMat",
            color = new Color(0.118f, 0.384f, 0.847f, 1f),
        };
        _kioskMaterial = mat;
        return mat;
    }

    private static Material GetSignMaterial()
    {
        if (_signMaterial != null) return _signMaterial;
        var shader = FindUsableShader();
        var mat = new Material(shader)
        {
            name = "HookGun_KioskSignMat",
            color = Color.white,
        };
        _signMaterial = mat;
        return mat;
    }

    private static Shader FindUsableShader()
    {
        return Shader.Find("Universal Render Pipeline/Lit")
            ?? Shader.Find("Standard")
            ?? Shader.Find("Sprites/Default");
    }

    public bool IsInteractible(Character interactor) => !ShopWindow.IsOpen;
    public bool IsConstantlyInteractable(Character interactor) => !ShopWindow.IsOpen;
    public float GetInteractTime(Character interactor) => InteractTime;

    public void Interact(Character interactor)
    {
        // никаких эффектов на старте удержания
    }

    public void Interact_CastFinished(Character interactor)
    {
        if (!ShopWindow.IsOpen)
        {
            ShopWindow.Toggle();
            Plugin.Log.LogInfo("Shop opened via kiosk interaction.");
        }
    }

    public void CancelCast(Character interactor) { }
    public void ReleaseInteract(Character interactor) { }
    public void HoverEnter() { }
    public void HoverExit() { }

    public Vector3 Center() => transform.position + new Vector3(0f, 0.7f, 0f);
    public Transform GetTransform() => transform;
    public string GetInteractionText() => "Open shop";
    public string GetName() => "SHOP";
}

internal class Billboard : MonoBehaviour
{
    public GameObject? mesh;

    private void LateUpdate()
    {
        var cam = Camera.main;
        if (cam == null || mesh == null) return;
        var dir = mesh.transform.position - cam.transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f) return;
        mesh.transform.rotation = Quaternion.LookRotation(dir.normalized);
    }
}
