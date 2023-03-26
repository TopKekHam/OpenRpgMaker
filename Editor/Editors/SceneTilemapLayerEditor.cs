using System.Numerics;

namespace SBEngine.Editor;

public class SceneTilemapLayerEditor
{

    private bool _resizing;
    private Vector2I _newSize;
    private string x = "", y = "";
    
    public void Update(string id, Editor editor, SceneEditor sceneEditor, IMGUI imgui, ref TileMapLayer selectedLayer)
    {
        imgui.BeginRows(0, ContentAlignment.START);
        
        // layer data

        if (selectedLayer != null)
        {   
            imgui.Gap(new Vector2(0, imgui.Style.FontSize));
            imgui.Text(selectedLayer.Name);

            {
                imgui.CapHeight(imgui.Style.ElementHeight);
                imgui.BeginColumns(0, ContentAlignment.MIDDLE);
                {
                    imgui.Text("sort layer", VColors.White, 0, 1.25f);
                    imgui.IntField(id + "_sort_layer", ref selectedLayer.SortLayer);
                }
                imgui.EndLayout();
            }

            if (_resizing)
            {
                imgui.CapHeight(imgui.Style.ElementHeight);
                imgui.BeginColumns(0, ContentAlignment.MIDDLE);
                {
                    imgui.Text("width", VColors.White, 0, 1.25f);
                    imgui.IntField(id + "_resizing_x", ref _newSize.X);
                }
                imgui.EndLayout();
                
                imgui.CapHeight(imgui.Style.ElementHeight);
                imgui.BeginColumns(0, ContentAlignment.MIDDLE);
                {
                    imgui.Text("height", VColors.White, 0, 1.25f);
                    imgui.IntField(id + "_resizing_y", ref _newSize.Y);
                }
                imgui.EndLayout();
                
                imgui.CapHeight(imgui.Style.ElementHeight);
                imgui.BeginColumns(0);
                {
                    if (imgui.Button(id + "_cancel_resize_button", "Cancel"))
                    {
                        _resizing = false;
                    }
                    
                    imgui.Gap(new Vector2(imgui.Style.FontSize));
                    
                    if (imgui.Button(id + "_resize_button", "Resize"))
                    {
                        _resizing = false;
                        selectedLayer.Resize(_newSize);
                    }
                }
                imgui.EndLayout();
                
            }
            else
            {
                if (imgui.Button(id + "_resize_button", "Resize"))
                {
                    _resizing = true;
                    _newSize = selectedLayer.Size;
                }
            }
            
            imgui.Gap(new Vector2(0, imgui.Style.ElementHeight));
        }
        

        // add layer
        
        if (imgui.Button("scene_editor_add_tile_map_layer", "Add Layer"))
        {
            var newLayer = new TileMapLayer(editor.Engine.AssetsDatabase);
            newLayer.Name = "New Layer" + (editor.Game.ActiveScene.TileLayers.Count + 1);
            editor.Game.ActiveScene.TileLayers.Add(newLayer);
        }
        
        // layers
        
        var layers = editor.Game.ActiveScene.TileLayers;
        
        for (int i = 0; i < layers.Count; i++)
        {
            if (imgui.Button("scene_editor_tile_map_layer_select:" + layers[i].GetHashCode(),
                    $"Layer: {layers[i].Name}"))
            {
                selectedLayer = layers[i];
                sceneEditor.SelectedTile = -1;
            }
        }

        imgui.EndLayout();
    }
    
}