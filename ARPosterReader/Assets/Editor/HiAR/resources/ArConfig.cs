using UnityEngine;

namespace hiscene {

    /// <summary>
    ///     default configs
    /// </summary>
    public class ArConfig : ScriptableObject {

        public string UserName = "default user";

        //[SerializeField] private string _passWord = "";

        public bool PovEnablep = false;

        public GameObject PovConfigsCenter;

        public int PovType;

        private static HiAREngine _engine;

        public static HiAREngine Engine {
            get { return _engine ?? (_engine = FindObjectOfType<HiAREngine>()); }
        }
    }
}
