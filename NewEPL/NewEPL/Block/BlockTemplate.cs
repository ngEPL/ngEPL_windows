using System;
using System.Windows.Controls;

namespace NewEPL {
    public class BlockTemplate : UserControl{

        static BlockTemplate() {
        }

        public static BlockTemplate CreateBlock(BlockTemplate b) {
            var ret = (BlockTemplate)Activator.CreateInstance(b.Content.GetType());
            return ret;
        }
    }
}
