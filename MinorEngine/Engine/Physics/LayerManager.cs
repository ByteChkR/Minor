using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Debug;
using Engine.Exceptions;

namespace Engine.Physics
{
    /// <summary>
    /// Static class that has the purpose of managing the layers for all the objects and gets queried by the renderer
    /// </summary>
    public static class LayerManager
    {
        /// <summary>
        /// The Layer to Name Register
        /// </summary>
        private static List<Tuple<Layer, string>> _internalLayerStore = new List<Tuple<Layer, string>>();

        /// <summary>
        /// The Name to Index(of _internalLayerStore) Register that enables fast reverse lookup
        /// </summary>
        private static Dictionary<string, int> _registeredLayers = new Dictionary<string, int>();


        /// <summary>
        /// Function to register a layer with a specified name
        /// </summary>
        /// <param name="name">Name of the layer</param>
        /// <param name="layer">The layer data</param>
        /// <returns></returns>
        public static int RegisterLayer(string name, Layer layer)
        {
            if (_registeredLayers.ContainsKey(name))
            {
                return _registeredLayers[name];
            }

            _internalLayerStore.Add(new Tuple<Layer, string>(layer, name));
            _registeredLayers.Add(name, _internalLayerStore.Count - 1);
            return _internalLayerStore.Count - 1;
        }

        /// <summary>
        /// Resolves layer id to Layer conversion
        /// </summary>
        /// <param name="layer">the layer index</param>
        /// <returns>The layer associated with the index</returns>
        private static Layer IDToInternalLayer(int layer)
        {
            if (layer >= 0 && layer < _internalLayerStore.Count)
            {
                return _internalLayerStore[layer].Item1;
            }

            var rec = _registeredLayers.Count > 0;
            Logger.Crash(new LayerNotFoundException(layer), rec);
            return _internalLayerStore[_registeredLayers.ElementAt(0).Value].Item1;
        }


        /// <summary>
        /// Returns the id of the layer associated with the name
        /// </summary>
        /// <param name="name">the name of the layer</param>
        /// <returns>The id of the layer; if not found -1</returns>
        public static int NameToLayer(string name)
        {
            if (_registeredLayers.ContainsKey(name))
            {
                return _registeredLayers[name];
            }

            return -1;
        }

        /// <summary>
        /// Returns the Layer Name from the Layer ID
        /// </summary>
        /// <param name="layer">The layer id</param>
        /// <returns>the name associated with the ID</returns>
        public static string LayerToName(int layer)
        {
            if (layer >= 0 && layer < _internalLayerStore.Count)
            {
                return _internalLayerStore[layer].Item2;
            }

            return "Not Found";
        }

        /// <summary>
        /// Disables the collisions between two layers
        /// </summary>
        /// <param name="layerA">Index of layer A</param>
        /// <param name="layerB">Index of layer B</param>
        public static void DisableCollisions(int layerA, int layerB)
        {
            var a = IDToInternalLayer(layerA);
            var b = IDToInternalLayer(layerB);
            Layer.DisableCollision(ref a, ref b);
        }

        /// <summary>
        /// Tests if the two colliders are allowed to collide
        /// </summary>
        /// <param name="layerA">Index of layer A</param>
        /// <param name="layerB">Index of layer B</param>
        public static bool AllowCollision(int layerA, int layerB)
        {
            return Layer.AllowCollision(IDToInternalLayer(layerA), IDToInternalLayer(layerB));
        }
    }
}