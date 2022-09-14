using System;
using BepInEx;
using Timber.Models;
using Timber.Services;
using Jotunn;
using Jotunn.Entities;
using Jotunn.Managers;
using Jotunn.Utils;
using UnityEngine;
using System.IO;
using System.Runtime.InteropServices;
using Jotunn.Configs;

namespace Timber
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(Jotunn.Main.ModGuid)]
    internal class TimberMod : BaseUnityPlugin
    {
        public const string PluginGUID = "kuarion.timber";
        public const string PluginName = "timber";
        public const string PluginVersion = "1.0.0";
        
        private AssetBundle _embeddedResourceBundle;

        // Como The Fang va a ser utilizado al agregarlo a la ObjectDB y a las recetas,
        // entonces en lugar de buscarlo en el _embeddedResourceBundle dos veces,
        // se almacena en una variable la primera vez y luego se reutilizara.
        private GameObject TheFang;
        private void Awake()
        {
            LoadAssetBundle();
            AddItems();
            AddCreatures();
            AddRecipes();
            AddConversions();
            AddPieces();
            UnloadAssetBundle();

            // Listen to event to know when all prefabs are registered
            // Configurar los droptable con los prefabs añadidos.
            PrefabManager.OnPrefabsRegistered += () =>
            {
                // The null check is necessary in case users remove the item from the config
                // var prefab = PrefabManager.Instance.GetPrefab("timberwolfmeat");
            };
        }

        private void LoadAssetBundle()
        {
            // Load asset bundle from embedded resources
            Jotunn.Logger.LogInfo($"Embedded resources: {string.Join(",", typeof(TimberMod).Assembly.GetManifestResourceNames())}");
            // Carga el AssetBundle "timber"
            _embeddedResourceBundle = AssetUtils.LoadAssetBundleFromResources("timber", typeof(TimberMod).Assembly);
        }

        private void UnloadAssetBundle()
        {
            _embeddedResourceBundle.Unload(false);
        }

        #region "Items"
        private void AddItems()
        {
            AddTimberWolfMeat();
            AddCookedTimberWolfMeat();
            AddTrophyTimberWolf();
            AddCrackedFang();
            AddTimberWolfPelt();
            AddTheFang();
        }
        
        private void AddTimberWolfMeat()
        {
            var prefab = _embeddedResourceBundle.LoadAsset<GameObject>("Assets/CustomItems/TimberWolfMeat.prefab");
            var customItem = new CustomItem(prefab, true);
            ItemManager.Instance.AddItem(customItem);
        }
        
        private void AddCookedTimberWolfMeat()
        {
            var prefab = _embeddedResourceBundle.LoadAsset<GameObject>("Assets/CustomItems/TimberWolfMeatCooked.prefab");
            var customItem = new CustomItem(prefab, true);
            ItemManager.Instance.AddItem(customItem);
        }
        
        private void AddTrophyTimberWolf()
        {
            var prefab = _embeddedResourceBundle.LoadAsset<GameObject>("Assets/CustomItems/TimberWolfTrophy.prefab");
            var customItem = new CustomItem(prefab, true);
            ItemManager.Instance.AddItem(customItem);
        }
        
        private void AddCrackedFang()
        {
            var prefab = _embeddedResourceBundle.LoadAsset<GameObject>("Assets/CustomItems/TimberWolfFang.prefab");
            var customItem = new CustomItem(prefab, true);
            ItemManager.Instance.AddItem(customItem);
        }
        
        private void AddTimberWolfPelt()
        {
            var prefab = _embeddedResourceBundle.LoadAsset<GameObject>("Assets/CustomItems/TimberWolfPelt.prefab");
            var customItem = new CustomItem(prefab, true);
            ItemManager.Instance.AddItem(customItem);
        }

        private void AddTheFang()
        {
            TheFang = _embeddedResourceBundle.LoadAsset<GameObject>("Assets/CustomItems/TheFang.prefab");
            var customItem = new CustomItem(TheFang, true);
            ItemManager.Instance.AddItem(customItem);
        }
        #endregion

        #region "Mesas"

        #endregion

        #region "Recetas"
        private void AddRecipes()
        {
            AddTheFangRecipe();
        }

        private void AddTheFangRecipe()
        {
            var fangRecipe = new RecipeConfig();
            fangRecipe.Item = "TheFang";
            fangRecipe.AddRequirement(new RequirementConfig("SwordIron",1));
            fangRecipe.AddRequirement(new RequirementConfig("TimberWolfFang",1));
            fangRecipe.CraftingStation = "Anima";
            ItemManager.Instance.AddRecipe(new CustomRecipe(fangRecipe));
        }
        #endregion
        
        #region "Creatures"
        private void AddCreatures()
        {
            AddTimberWolf();
            //AddSuperDraugr();
        }
        // Agrega a una criatura desde un AssetBundle.
        private void AddTimberWolf()
        {
            var prefab = _embeddedResourceBundle.LoadAsset<GameObject>("Assets/CustomItems/TimberWolf.prefab");
            var creature = new CustomCreature(prefab, true);
            CreatureManager.Instance.AddCreature(creature);
        }
        
        // Clona a un draugr normal desde los archivos del juego.
        // Cuadriplica su salud, tamaño.
        // Disminuye a la mitad su velocidad de movimiento.
        private void AddSuperDraugr()
        {
            var superGreydwarfConfig = new CreatureConfig();
            // TODO : Hacerlos spawnear aleatoriamente en un bioma.
            var superGreydwarf = new CustomCreature("SuperGreydwarf","Greydwarf", superGreydwarfConfig);
            var humanoid = superGreydwarf.Prefab.GetComponent<Humanoid>();
            humanoid.m_name = "Super Greydwarf";
            humanoid.m_health *= 4;
            humanoid.m_walkSpeed *= 0.5f;
            humanoid.m_runSpeed *= 0.5f;
            superGreydwarf.Prefab.transform.localScale = new Vector3(4, 4, 4);
            CreatureManager.Instance.AddCreature(superGreydwarf);
        }
        #endregion
        
        #region "Conversions"
        private void AddConversions()
        {
            AddTimberWolfMeatConversion();
        }
        
        // Permite poner TimberWolfMeat en una cooking station.
        private void AddTimberWolfMeatConversion()
        {
            var config = new CookingConversionConfig();
            config.FromItem = "TimberWolfMeat";
            config.ToItem = "CookedTimberWolfMeat";
            config.CookTime = 25;
            config.Station = "piece_cookingstation";
            var conversion = new CustomItemConversion(config);
            ItemManager.Instance.AddItemConversion(conversion);
        }
        #endregion

        #region "Pieces"
        private void AddPieces()
        {
            AddAnima();
        }

        private void AddAnima()
        {
            PieceConfig pieceConfig = new PieceConfig();
            pieceConfig.PieceTable = "Hammer";
            RequirementConfig pieceRequirementConfig = new RequirementConfig("Wood",2,0,false);
            pieceConfig.AddRequirement(pieceRequirementConfig);
            PieceManager.Instance.AddPiece(new CustomPiece(_embeddedResourceBundle, "Anima", pieceConfig));
        }
        #endregion
    }
}