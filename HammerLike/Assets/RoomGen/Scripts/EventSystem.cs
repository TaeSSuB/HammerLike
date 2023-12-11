using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;


namespace RoomGen
{
    public class EventSystem : MonoBehaviour
    {

        private static EventSystem _instance;

        public static EventSystem instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = (EventSystem)FindObjectOfType(typeof(EventSystem));
                    if (_instance == null)
                    {
                        var singletonObject = new GameObject();
                        _instance = singletonObject.AddComponent<EventSystem>();
                        singletonObject.name = typeof(EventSystem).ToString() + " (Singleton)";
                    }
                }

                return _instance;
            }
        }

        ///The main function that will be invoked to regenerate a room. Should be called after all RoomGenerator component variables have been adjusted.
        public event Action OnGenerate;

        ///Updating these values will increase or decrease the size of the room.
        public event Action<int, int> OnSetGridSize;

        ///Update the room seed. This alters the doors, floors, and wall tiles and their locations. Characters and decorations will not be affected.
        public event Action<int, int> OnSetRoomSeed;
        ///Update the Decor seed. This alters the roof, floor, and wall decorations and their locations. The wall/floor tiles themselves will not be affected.
        public event Action<int, int> OnSetDecorSeed;
        ///Update the Character seed. This alters the characters and their locations.
        public event Action<int, int> OnSetCharacterSeed;

        public event Action<int, int> OnSetDoorCount;
        public event Action<int, int> OnSetWindowCount;
        public event Action<int, int> OnSetLevelHeight;

        public event Action<int, Vector3> OnSetLevelOffset;
        
        
        //Target Generator Event Actions
        ///The main function that will be invoked to regenerate a Target Generator. Should be called after all TargetGenerator component variables have been adjusted.
        public event Action OnGenerateTarget;
        
        /// Set the radius of the target generator. 
        public event Action<int, float> OnSetTargetGenRadius;
        
        /// Set the seed of the target generator. 
        public event Action<int, int> OnSetTargetGenSeed;
        
        /// Set the object density of the target generator. 
        public event Action<int, int> OnSetTargetGenObjectDensity;
        


        private void Awake()
        {
            var instance = EventSystem.instance;
        }


        public void Generate()
        {
            OnGenerate?.Invoke();
        }

        public void SetGridSize(int x, int z)
        {
            OnSetGridSize?.Invoke(x, z);
        }

        public void SetRoomSeed(int levelNumber, int seed)
        {
            OnSetRoomSeed?.Invoke(levelNumber, seed);
        }

        public void SetDecorSeed(int levelNumber, int seed)
        {
            OnSetDecorSeed?.Invoke(levelNumber, seed);
        }

        public void SetCharacterSeed(int levelNumber, int seed)
        {
            OnSetCharacterSeed?.Invoke(levelNumber, seed);
        }

        public void SetDoorCount(int levelNumber, int count)
        {
            OnSetDoorCount?.Invoke(levelNumber, count);
        }

        public void SetWindowCount(int levelNumber, int count)
        {
            OnSetWindowCount?.Invoke(levelNumber, count);
        }

        public void SetLevelHeight(int levelNumber, int height)
        {
            OnSetLevelHeight?.Invoke(levelNumber, height);
        }

        public void SetLevelOffset(int levelNumber, Vector3 offset)
        {
            OnSetLevelOffset?.Invoke(levelNumber, offset);
        }
        
        
        
        // Target Generator Events

        public void GenerateTargetGen()
        {
            OnGenerateTarget?.Invoke();
        }
        
        public void SetTargetGenRadius(int generatorID, float radius)
        {
            OnSetTargetGenRadius?.Invoke(generatorID, radius);
        }

        public void SetTargetGenSeed(int generatorID, int seed)
        {
            OnSetTargetGenSeed?.Invoke(generatorID, seed);
        }
        
        public void SetTargetGenObjectDensity(int generatorID, int objectDensity)
        {
            OnSetTargetGenObjectDensity?.Invoke(generatorID, objectDensity);
        }
        
    }
}

