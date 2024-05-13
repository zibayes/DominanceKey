using UnityEngine;
using UnityEngine.UI;

namespace VariableCode.Cursor
{
    public class CursorSwitcher : MonoBehaviour
    {
        public CursorId[] cursors;
        public CursorId current;


        public void ChangeType(string name)
        {
            int cursorId = 0;
            if (name == "collect")
                cursorId = 1;
            else if (name == "fire")
                cursorId = 2;
            else if (name == "aim")
                cursorId = 3;
            else if (name == "reload")
                cursorId = 4;
            else if (name == "rotate")
                cursorId = 5;
            if (current.objectIndex != cursorId)
            {
                current = cursors[cursorId];
                CursorManager.EnsureCreation().SetActiveCursor(current);
            }
        }
    }
}
