using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class ModificationBuffer : MonoBehaviour
{
    const int ModBufferSize = 1024;
    RenderTexture m_ModBuffer;

    CommandBuffer m_commandBuffer;
    Camera m_activeCamera;

    List<Renderer> m_modRenderers;

    private void Awake()
    {
        m_activeCamera = Camera.current;
        m_modRenderers = new List<Renderer>();
        GatherRenderers();
        SetupResources();
    }

    private void OnDisable()
    {
        TearDownResources();
    }

    private void Update()
    {
        GatherRenderers();
    }

    private void GatherRenderers()
    {
        m_modRenderers.Clear();
        GameObject[] allMods = GameObject.FindGameObjectsWithTag("Mod");
        foreach(GameObject mod in allMods)
        {
            Renderer renderer = mod.GetComponent<Renderer>();
            if(renderer != null)
                m_modRenderers.Add(renderer);
        }
        SetupCommandBuffer();
    }

    private void SetupResources()
    {
        m_ModBuffer = new RenderTexture(ModBufferSize, ModBufferSize, 16, RenderTextureFormat.R8);
        m_commandBuffer = new CommandBuffer();

        SetupCommandBuffer();

        if (m_activeCamera)
        {
            m_activeCamera.AddCommandBuffer(CameraEvent.BeforeGBuffer, m_commandBuffer);
        }
    }

    private void SetupCommandBuffer()
    {
        if (m_commandBuffer == null)
            return;

        m_commandBuffer.Clear();
        m_commandBuffer.name = "Modificaiton Buffer";
        m_commandBuffer.SetRenderTarget(m_ModBuffer);
        foreach(Renderer renderer in m_modRenderers)
            m_commandBuffer.DrawRenderer(renderer, renderer.material, 0, 1);
    }

    private void TearDownResources()
    {
        if (m_activeCamera)
            m_activeCamera.RemoveCommandBuffer(CameraEvent.BeforeGBuffer, m_commandBuffer);

        if (m_ModBuffer != null)
        {
            m_ModBuffer.DiscardContents();
            if (!Application.isPlaying)
                DestroyImmediate(m_ModBuffer);
            else
                Destroy(m_ModBuffer);
        }

        if (m_commandBuffer != null)
        {
            m_commandBuffer.Release();
            m_commandBuffer.Dispose();
        }
    }
}
