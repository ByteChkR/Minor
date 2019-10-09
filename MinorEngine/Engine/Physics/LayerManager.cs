﻿using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Debug;
using Engine.Exceptions;

namespace Engine.Physics
{
    public static class LayerManager
    {
        private static List<Tuple<Layer, string>> _internalLayerStore = new List<Tuple<Layer, string>>();
        private static Dictionary<string, int> _registeredLayers = new Dictionary<string, int>();


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

        public static int NameToLayer(string name)
        {
            if (_registeredLayers.ContainsKey(name))
            {
                return _registeredLayers[name];
            }

            return -1;
        }

        public static string LayerToName(int layer)
        {
            if (layer >= 0 && layer < _internalLayerStore.Count)
            {
                return _internalLayerStore[layer].Item2;
            }

            return "Not Found";
        }

        public static void DisableCollisions(int layerA, int layerB)
        {
            var a = IDToInternalLayer(layerA);
            var b = IDToInternalLayer(layerB);
            Layer.DisableCollision(ref a, ref b);
        }

        public static bool AllowCollision(int layerA, int layerB)
        {
            return Layer.AllowCollision(IDToInternalLayer(layerA), IDToInternalLayer(layerB));
        }
    }
}