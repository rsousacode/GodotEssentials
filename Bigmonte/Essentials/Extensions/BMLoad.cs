using Godot;

namespace Bigmonte.Essentials
{
    public static class BMLoad
    {
        public static Node PackedSceneInstance(string path)
        {
            var p = GD.Load(path) as PackedScene;
            return p?.Instance();
        }

        public static Node Instantiate(this Node node, string path)
        {
            var v = PackedSceneInstance(path);
            node.AddChild(v);
            return v;
        }

        public static Node Instantiate(this Node node, Resource resource)
        {
            var v = PackedSceneInstance(resource.ResourcePath);
            node.AddChild(v);
            return v;
        }


        public static Node Instantiate(this Node node, string path, Node parent)
        {
            var v = PackedSceneInstance(path);
            parent.AddChild(v);
            return v;
        }

        public static T Instantiate<T>(this Node node, string path, Node parent) where T : Node
        {
            var v = PackedSceneInstance(path);
            parent.AddChild(v);
            return v as T;
        }

        public static T Instantiate<T>(this Node node, Resource resource) where T : Node
        {
            var vnn = PackedSceneInstance(resource.ResourcePath);
            node.AddChild(vnn);
            return vnn as T;
        }

        public static Node Instantiate(this Node node, PackedScene packageScene)
        {
            var i = packageScene.Instance();
            node.AddChild(i);
            return i;
        }

        public static Panel InstantiatePanel(this Node node, PackedScene packageScene,
            Vector2 relativePosition = default)
        {
            var instance = node.Instantiate(packageScene);
            var panel = instance.GetComponent<Panel>();
            if (panel == null) return null;
            var size = panel.RectSize;
            var newPosX = panel.RectSize.x - size.x / 2;
            var newPosY = panel.RectSize.y - size.y / 2;
            var newPos = new Vector2(newPosX, newPosY) + relativePosition;
            panel.SetPosition(newPos);
            panel.SetActive(true);
            return panel;
        }

        public static T InstantiatePanel<T>(this Node node, PackedScene packageScene,
            Vector2 relativePosition = default) where T : Node
        {
            var instance = node.Instantiate(packageScene);
            var panel = instance.GetComponent<Panel>();
            if (panel == null) return null;
            var size = panel.RectSize;
            var newPosX = panel.RectPosition.x - size.x / 2;
            var newPosY = panel.RectPosition.y - size.y / 2;
            var newPos = new Vector2(newPosX, newPosY) + relativePosition;
            panel.SetPosition(newPos);
            panel.SetActive(true);
            return panel as T;
        }

        public static T InstantiatePanel<T>(this Node node, string resourcePath, Vector2 relativePosition = default)
            where T : Node
        {
            var instance = node.Instantiate(resourcePath);
            var panel = instance.GetComponent<Panel>();
            if (panel == null) return null;
            var size = panel.RectSize;
            var newPosX = panel.RectPosition.x - size.x / 2;
            var newPosY = panel.RectPosition.y - size.y / 2;
            var newPos = new Vector2(newPosX, newPosY) + relativePosition;
            panel.SetPosition(newPos);
            panel.SetActive(true);
            return panel as T;
        }

        public static Panel InstantiatePanel(this Node node, string path, Vector2 relativePosition = default)
        {
            var instance = node.Instantiate(path);
            var panel = instance.GetComponent<Panel>();
            if (panel == null) return null;
            var size = panel.RectSize;
            var newPosX = panel.RectPosition.x - size.x / 2;
            var newPosY = panel.RectPosition.y - size.y / 2;
            var newPos = new Vector2(newPosX, newPosY) + relativePosition;
            panel.SetPosition(newPos);
            panel.SetActive(true);
            return panel;
        }
    }
}