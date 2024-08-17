using System.Numerics;

namespace Concrete;

public class ModelRenderer : Component
{
    private Model model;
    private Shader shader = Shader.Default;

    [Include]
    public string modelPath
    {
        get => currentModelPath;
        set
        {
            currentModelPath = value;
            model = ModelReader.Load(currentModelPath);
        }
    }
    private string currentModelPath;

    public override void Render(float deltaTime, Projection projection)
    {
        shader.Use();

        shader.SetMatrix4("model", gameObject.transform.GetWorldModelMatrix());
        shader.SetMatrix4("view", projection.view);
        shader.SetMatrix4("proj", projection.proj);
        
        shader.SetLights(SceneManager.loadedScene.FindActiveLights());
        
        foreach (var mesh in model.meshes)
        {
            var material = model.materials[(int)mesh.materialIndex];

            shader.SetVector4("matColor", material.color);

            var hasAlbedo = material.albedoTexture != null;
            shader.SetBool("matHasAlbedoTexture", hasAlbedo);
            if (hasAlbedo) shader.SetTexture("matAlbedoTexture", (uint)material.albedoTexture, 2);

            var hasRoughness = material.roughnessTexture != null;
            shader.SetBool("matHasRoughnessTexture", hasRoughness);
            if (hasRoughness) shader.SetTexture("matRoughnessTexture", (uint)material.roughnessTexture, 3);

            mesh.Render();
        }
    }
}