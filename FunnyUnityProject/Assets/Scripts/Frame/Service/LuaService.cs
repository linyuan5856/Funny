using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using XLua;
using XLua.LuaDLL;
using LuaSvr = XLua.LuaEnv;
using LuaDLL = XLua.LuaDLL.Lua;
using RealStatePtr = System.IntPtr;
using LuaCSFunction = XLua.LuaDLL.lua_CSFunction;

namespace GFrame.Service
{
    public class LuaService : BaseService
    {
        private LuaEnv _luaEnv;
        public LuaEnv ENV => _luaEnv;

        public static LuaService Instance = new LuaService();

        public bool IsInited { get; private set; }

        private double _initProgress = 0;

        public double InitProgress => _initProgress;


        /// <summary>
        /// 是否开启缓存模式，默认true，首次执行将把执行结果table存起来；在非缓存模式下，可以通过编辑器的Reload来进行强制刷新缓存
        /// 对实时性重载要求高的，可以把开关设置成false，长期都进行Lua脚本重载，理论上会消耗额外的性能用于语法解析
        /// 
        /// 一般的脚本语言，如Python, NodeJS中，其import, require关键字都会对加载过的模块进行缓存(包括Lua原生的require)；如果不缓存，要注意状态的保存问题
        /// 该值调用频繁，就不放ini了
        /// </summary>
        public static bool CacheMode = true;

        /// <summary>
        /// Import result object caching
        /// </summary>
        Dictionary<string, object> _importCache = new Dictionary<string, object>();

        protected override void OnCreate()
        {
            base.OnCreate();
#if UNITY_EDITOR
            UnityEngine.Debug.Log("Consturct LuaModule...");
#endif
            _luaEnv = new LuaEnv();
        }


        public IEnumerator Init()
        {
            var L = _luaEnv.L;
            //在lua G中增加import函数
            LuaDLL.lua_pushstdcallcfunction(L, LuaImport);
            LuaDLL.xlua_setglobal(L, "import");

            //TODO lua中需要require的第三方库加到这里，如果不需要则删除已添加的这几行
            _luaEnv.AddBuildin("rapidjson", XLua.LuaDLL.Lua.LoadRapidJson);
            _luaEnv.AddBuildin("lpeg", XLua.LuaDLL.Lua.LoadLpeg);
            _luaEnv.AddBuildin("pb", XLua.LuaDLL.Lua.LoadLuaProfobuf);
            _luaEnv.AddBuildin("ffi", XLua.LuaDLL.Lua.LoadFFI);
            yield return null;
            CallScript("Init");
            IsInited = true;
        }

        public void ClearData()
        {
            ClearAllCache();
        }

        #region c/csharp import

        [MonoPInvokeCallback(typeof(lua_CSFunction))]
        internal static int LuaImport(IntPtr L)
        {
            LuaService service = ServiceLocate.Get().GetService<LuaService>();
            string fileName = Lua.lua_tostring(L, 1);
            var obj = service.Import(fileName);

            ObjectTranslator ot = ObjectTranslatorPool.Instance.Find(L);
            ot.PushAny(L, obj);
            ot.PushAny(L, true);

            return 2;
        }

        #endregion

        /// <summary>
        /// Execute lua script directly!
        /// </summary>
        /// <param name="scriptCode"></param>
        /// <param name="ret">return result</param>
        /// <returns></returns>
        public bool ExecuteScript(byte[] scriptCode, out object ret, string file = "chunk")
        {
            var results = _luaEnv.DoString(Encoding.UTF8.GetString(scriptCode), file);

            if (results != null && results.Length == 1)
                ret = results[0];
            else
                ret = results;
            return true;
        }

        /// <summary>
        /// Execute lua script directly!
        /// </summary>
        /// <param name="scriptCode"></param>
        /// <returns></returns>
        public object ExecuteScript(byte[] scriptCode, string file = "chunk")
        {
            ExecuteScript(scriptCode, out var ret, file);
            return ret;
        }

        /// <summary>
        /// Get script full path
        /// </summary>
        /// <param name="scriptRelativePath"></param>
        /// <returns></returns>
        static string GetScriptPath(string scriptRelativePath)
        {
            return string.Format("{0}/{1}.lua", "Lua", scriptRelativePath);
        }

        /// <summary>
        /// whether the script file exists?
        /// </summary>
        /// <param name="scriptRelativePath"></param>
        /// <returns></returns>
        public bool HasScript(string scriptRelativePath)
        {
            var scriptPath = GetScriptPath(scriptRelativePath);
            return LoaderService.IsResourceExist(scriptPath);
        }

        /// <summary>
        /// Call script of script path (relative) specify
        /// 
        /// We don't recommend use this method, please use ImportScript which has Caching!
        /// </summary>
        /// <param name="scriptRelativePath"></param>
        /// <returns></returns>
        public object CallScript(string scriptRelativePath)
        {
            if (scriptRelativePath == null || string.IsNullOrEmpty(scriptRelativePath))
                return null;
            
            var scriptPath = GetScriptPath(scriptRelativePath);
            if (!LoaderService.IsResourceExist(scriptPath)) 
                return null;
            
            byte[] script =LoaderService.LoadAssetsSync(scriptPath);
            UnityEngine.Debug.Assert(script != null, $"ExecuteScript error,script byte null,path:{scriptPath}");
            var ret = ExecuteScript(script, scriptRelativePath);
            return ret;
        }

        #region import lua file

        /// <summary>
        /// Import script, with caching
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public object Import(string fileName)
        {
            object obj;

            //NOTE 优先从cache获取
            if (CacheMode && _importCache.TryGetValue(fileName, out obj))
            {
                return obj;
            }

            if (!HasScript(fileName))
                throw new FileNotFoundException(string.Format("Not found Lua Script: {0}", fileName));

            return DoImportScript(fileName);
        }

        /// <summary>
        /// Try import script, if 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool TryImport(string fileName, out object result)
        {
            result = null;

            if (!HasScript(fileName))
            {
               UnityEngine.Debug.LogError($"{fileName} not exist !");
                return false;
            }

            result = DoImportScript(fileName);
            return true;
        }

        object DoImportScript(string fileName)
        {
            object obj;
            if (CacheMode)
            {
                if (!_importCache.TryGetValue(fileName, out obj))
                {
                    obj = CallScript(fileName);
                    _importCache[fileName] = obj;
                }
            }
            else
            {
                obj = CallScript(fileName);
            }

            return obj;
        }


        /// <summary>
        /// Clear all imported cache
        /// </summary>
        public void ClearAllCache()
        {
            if (!CacheMode) return;
            _importCache.Clear();
            //if (AppConfig.isEditor) Log.Info("Call Clear All Lua Import Cache");
        }

        /// <summary>
        /// Clear dest lua script cache
        /// </summary>
        /// <param name="uiLuaPath"></param>
        /// <returns></returns>
        public bool ClearCache(string uiLuaPath)
        {
            if (!CacheMode) return false;
            return _importCache.Remove(uiLuaPath);
        }

        #endregion
    }
}