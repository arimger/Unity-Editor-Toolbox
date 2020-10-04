using System;
using System.Collections;

namespace Toolbox.Editor.Routine
{
    public static class EditorCoroutineUtility
    {
        public static EditorCoroutine StartCoroutine(IEnumerator routine, object owner)
        {
            return new EditorCoroutine(routine, owner);
        }

        public static EditorCoroutine StartCoroutine(IEnumerator routine)
        {
            return new EditorCoroutine(routine);
        }

        public static void StopCoroutine(EditorCoroutine coroutine)
        {
            if (coroutine == null)
            {
                throw new ArgumentNullException(nameof(coroutine));
            }

            coroutine.Stop();
        }
    }
}