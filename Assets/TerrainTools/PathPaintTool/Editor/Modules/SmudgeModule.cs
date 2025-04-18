﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor.TerrainTools;
using UnityEngine;



namespace UnityEditor.Experimental.TerrainAPI
{
    public class SmudgeModule : ModuleEditor
    {

        #region Materials
        Material material = null;
        Material GetMaterial()
        {
            if (material == null)
                material = new Material(Shader.Find("Hidden/TerrainTools/PathPaintTool/SmudgeHeight"));
            return material;
        }
        #endregion Materials

        #region Fields

        [SerializeField]
        float smudgeBrushSize = 150f;

        [SerializeField]
        float smudgeBrushStrength = 20f;

        #endregion Fields

        private static Color smudgeBrushColor = new Color(0.5f, 0.7f, 0.5f, 0.8f);

        override public string GetName()
        {
            return "Smudge";
        }

        override public string GetDescription()
        {
            return "";
        }


        override public void OnSceneGUI(Terrain currentTerrain, IOnSceneGUI editContext)
        {
            if (editContext.hitValidTerrain)
            {
                Terrain terrain = currentTerrain;

                // the smooth brush size is relative to the main brush size
                float brushSize = editContext.brushSize * this.smudgeBrushSize / 100f;

                UnityEngine.TerrainTools.BrushTransform brushXform = UnityEngine.TerrainTools.TerrainPaintUtility.CalculateBrushTransform(terrain, editContext.raycastHit.textureCoord, brushSize, 0.0f);
                UnityEngine.TerrainTools.PaintContext ctx = UnityEngine.TerrainTools.TerrainPaintUtility.BeginPaintHeightmap(terrain, brushXform.GetBrushXYBounds(), 1);
                Material brushPreviewMat = BrushUtilities.GetDefaultBrushPreviewMaterial();
                brushPreviewMat.color = smudgeBrushColor;
                BrushUtilities.DrawBrushPreview(ctx, BrushUtilities.BrushPreview.SourceRenderTexture, editContext.brushTexture, brushXform, brushPreviewMat, 0);
                UnityEngine.TerrainTools.TerrainPaintUtility.ReleaseContextResources(ctx);
            }
        }

        override public void OnInspectorGUI(Terrain terrain, IOnInspectorGUI editContext)
        {
            EditorGUILayout.LabelField("Smudge", EditorStyles.boldLabel);

            smudgeBrushSize = EditorGUILayout.Slider(new GUIContent("Brush Size [% of Main Brush]", ""), smudgeBrushSize, 0.0f, 300.0f);
            smudgeBrushStrength = EditorGUILayout.Slider(new GUIContent("Brush Strength", ""), smudgeBrushStrength, 0.0f, 100.0f);
        }

        override public void PaintSegments(StrokeSegment[] segments, IOnPaint editContext)
        {
            for (int i = 0; i < segments.Length; i++)
            {
                // first item doesn't have prevUV
                if (i == 0)
                    continue;

                StrokeSegment segment = segments[i];

                Smudge(segment.currTerrain, editContext, segment.currUV, segment.prevUV);
            }
        }


        private bool Smudge(Terrain terrain, IOnPaint editContext, Vector2 currUV, Vector2 prevUV)
        {
            // the brush size is relative to the main brush size
            float brushSize = editContext.brushSize * this.smudgeBrushSize / 100f;

            UnityEngine.TerrainTools.BrushTransform brushXform = UnityEngine.TerrainTools.TerrainPaintUtility.CalculateBrushTransform(terrain, currUV, brushSize, 0.0f);
            UnityEngine.TerrainTools.PaintContext paintContext = UnityEngine.TerrainTools.TerrainPaintUtility.BeginPaintHeightmap(terrain, brushXform.GetBrushXYBounds(), 1);

            Vector2 smudgeDir = editContext.uv - prevUV;

            paintContext.sourceRenderTexture.filterMode = FilterMode.Bilinear;

            Material mat = GetMaterial();

            float brushStrength = this.smudgeBrushStrength / 100f;

            Vector4 brushParams = new Vector4(brushStrength, smudgeDir.x, smudgeDir.y, 0);
            mat.SetTexture("_BrushTex", editContext.brushTexture);
            mat.SetVector("_BrushParams", brushParams);
            UnityEngine.TerrainTools.TerrainPaintUtility.SetupTerrainToolMaterialProperties(paintContext, brushXform, mat);
            Graphics.Blit(paintContext.sourceRenderTexture, paintContext.destinationRenderTexture, mat, 0);

            UnityEngine.TerrainTools.TerrainPaintUtility.EndPaintHeightmap(paintContext, "Terrain Paint - Smudge Height");

            return true;
        }
    }
}
