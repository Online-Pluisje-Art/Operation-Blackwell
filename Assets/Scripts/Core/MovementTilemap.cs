using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationBlackwell.Core {
    public class MovementTilemap {

        public event EventHandler OnLoaded;

        private Grid<TilemapObject> grid;

        public MovementTilemap(int width, int height, float cellSize, Vector3 originPosition) {
            grid = new Grid<TilemapObject>(width, height, cellSize, originPosition, (Grid<TilemapObject> g, Vector3 v, int x, int y) => new TilemapObject(g, x, y));
        }

        public void SetTilemapSprite(Vector3 worldPosition, TilemapObject.TilemapSprite tilemapSprite) {
            TilemapObject tilemapObject = grid.NodeFromWorldPoint(worldPosition);
            if (tilemapObject != null) {
                tilemapObject.SetTilemapSprite(tilemapSprite);
            }
        }

        public void SetTilemapSprite(int x, int y, TilemapObject.TilemapSprite tilemapSprite) {
            TilemapObject tilemapObject = grid.NodeFromWorldPoint(new Vector3(x, y));
            if (tilemapObject != null) {
                tilemapObject.SetTilemapSprite(tilemapSprite);
            }
        }

        public void SetAllTilemapSprite(TilemapObject.TilemapSprite tilemapSprite) {
            for (int x = 0; x < grid.gridSizeX; x++) {
                for (int y = 0; y < grid.gridSizeY; y++) {
                    SetTilemapSprite(x, y, tilemapSprite);
                }
            }
        }

        public void SetTilemapVisual(MovementTilemapVisual tilemapVisual) {
            tilemapVisual.SetGrid(this, grid);
        }



        /*
        * Save - Load
        * */
        public class SaveObject {
            public TilemapObject.SaveObject[] tilemapObjectSaveObjectArray;
        }

        public void Save() {
            List<TilemapObject.SaveObject> tilemapObjectSaveObjectList = new List<TilemapObject.SaveObject>();
            for (int x = 0; x < grid.gridSizeX; x++) {
                for (int y = 0; y < grid.gridSizeY; y++) {
                    TilemapObject tilemapObject = grid.NodeFromWorldPoint(new Vector3(x, y));
                    tilemapObjectSaveObjectList.Add(tilemapObject.Save());
                }
            }

            SaveObject saveObject = new SaveObject { tilemapObjectSaveObjectArray = tilemapObjectSaveObjectList.ToArray() };

            // SaveSystem.SaveObject(saveObject);
        }

        public void Load() {
            // SaveObject saveObject = SaveSystem.LoadMostRecentObject<SaveObject>();
            // foreach (TilemapObject.SaveObject tilemapObjectSaveObject in saveObject.tilemapObjectSaveObjectArray) {
            //     TilemapObject tilemapObject = grid.NodeFromWorldPoint(new Vector3(tilemapObjectSaveObject.x, tilemapObjectSaveObject.y));
            //     tilemapObject.Load(tilemapObjectSaveObject);
            // }
            // OnLoaded?.Invoke(this, EventArgs.Empty);
        }



        /*
        * Represents a single Tilemap Object that exists in each Grid Cell Position
        * */
        public class TilemapObject {

            public enum TilemapSprite {
                None,
                Move,
            }

            private Grid<TilemapObject> grid;
            private int x;
            private int y;
            private TilemapSprite tilemapSprite;

            public TilemapObject(Grid<TilemapObject> grid, int x, int y) {
                this.grid = grid;
                this.x = x;
                this.y = y;
            }

            public void SetTilemapSprite(TilemapSprite tilemapSprite) {
                this.tilemapSprite = tilemapSprite;
                grid.TriggerGridObjectChanged(x, y);
            }

            public TilemapSprite GetTilemapSprite() {
                return tilemapSprite;
            }

            public override string ToString() {
                return tilemapSprite.ToString();
            }



            [System.Serializable]
            public class SaveObject {
                public TilemapSprite tilemapSprite;
                public int x;
                public int y;
            }

            /*
            * Save - Load
            * */
            public SaveObject Save() {
                return new SaveObject { 
                    tilemapSprite = tilemapSprite,
                    x = x,
                    y = y,
                };
            }

            public void Load(SaveObject saveObject) {
                tilemapSprite = saveObject.tilemapSprite;
            }
        }

    }
}
