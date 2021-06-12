using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;

class ModPass : CustomPass
{
    const int ModBufferSize = 1024;
    RenderTexture m_ModBuffer;

    protected override void Setup(ScriptableRenderContext renderContext, CommandBuffer cmd)
    {
       
            /*RTHandles.Alloc(
        Vector2.one, TextureXR.slices, dimension: TextureXR.dimension,
        colorFormat: GraphicsFormat.R8_UNorm,
        useDynamicScale: true, name: "Mod Buffer"
        ); */
    }


    protected override void Execute(CustomPassContext ctx)
    {
        var size = ctx.cameraDepthBuffer.GetScaledSize(ctx.cameraDepthBuffer.referenceSize);
        if (null == m_ModBuffer || m_ModBuffer.width != size.x || m_ModBuffer.height != size.y)
        {
            if(null!= m_ModBuffer)
            {
                m_ModBuffer.DiscardContents();
                m_ModBuffer.Release();
                m_ModBuffer = null;
            }
            m_ModBuffer = new RenderTexture(size.x, size.y, 0, RenderTextureFormat.R8);
        }

        ShaderTagId shaderTagId = new ShaderTagId("Mod");

        CoreUtils.SetRenderTarget(ctx.cmd, m_ModBuffer, ctx.cameraDepthBuffer, ClearFlag.Color, Color.black);
        RendererListDesc renderListDesc = new RendererListDesc(shaderTagId, ctx.cullingResults, ctx.hdCamera.camera)
        {
            renderQueueRange = RenderQueueRange.all,
            sortingCriteria = SortingCriteria.BackToFront,
            excludeObjectMotionVectors = false,
        };
        CoreUtils.DrawRendererList(ctx.renderContext, ctx.cmd, RendererList.Create(renderListDesc));
        CoreUtils.SetRenderTarget(ctx.cmd, ctx.cameraColorBuffer, ctx.cameraDepthBuffer);
        ctx.cmd.Clear();
        ctx.cmd.SetGlobalTexture("g_ModBuffer", m_ModBuffer);
        ctx.renderContext.ExecuteCommandBuffer(ctx.cmd);
    }

    protected override void Cleanup()
    {
        m_ModBuffer.Release();
    }

}