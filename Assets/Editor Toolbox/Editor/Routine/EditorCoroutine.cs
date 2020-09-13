using System;
using System.Collections;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Routine
{
    //NOTE: implementation based on the EditorCoroutine package 

    public class EditorCoroutine
    {
        private struct YieldProcessor
        {
            private enum DataType : byte
            {
                None = 0,
                WaitForSeconds = 1,
                EditorCoroutine = 2,
                AsyncOp = 3,
            }

            private struct ProcessorData
            {
                public DataType type;
                public double targetTime;
                public object current;
            }

            private ProcessorData data;

            public void Set(object yield)
            {
                if (yield == data.current)
                {
                    return;
                }

                var type = yield.GetType();
                var dataType = DataType.None;
                double targetTime = -1;

                if (type == typeof(EditorWaitForSeconds))
                {
                    targetTime = EditorApplication.timeSinceStartup + (yield as EditorWaitForSeconds).WaitTime;
                    dataType = DataType.WaitForSeconds;
                }
                else if (type == typeof(EditorCoroutine))
                {
                    dataType = DataType.EditorCoroutine;
                }
                else if (type == typeof(AsyncOperation) || type.IsSubclassOf(typeof(AsyncOperation)))
                {
                    dataType = DataType.AsyncOp;
                }

                data = new ProcessorData { current = yield, targetTime = targetTime, type = dataType };
            }

            public bool MoveNext(IEnumerator enumerator)
            {
                bool advance = false;
                switch (data.type)
                {
                    case DataType.WaitForSeconds:
                        advance = data.targetTime <= EditorApplication.timeSinceStartup;
                        break;
                    case DataType.EditorCoroutine:
                        advance = (data.current as EditorCoroutine).isDone;
                        break;
                    case DataType.AsyncOp:
                        advance = (data.current as AsyncOperation).isDone;
                        break;
                    default:
                        advance = data.current == enumerator.Current;
                        break;
                }

                if (advance)
                {
                    data = default;
                    return enumerator.MoveNext();
                }
                return true;
            }
        }


        internal WeakReference owner;

        private IEnumerator routine;
        private YieldProcessor processor;
        private bool isDone;


        internal EditorCoroutine(IEnumerator routine)
        {
            owner = null;
            this.routine = routine;
            EditorApplication.update += MoveNext;
        }

        internal EditorCoroutine(IEnumerator routine, object owner)
        {
            processor = new YieldProcessor();
            this.owner = new WeakReference(owner);
            this.routine = routine;
            EditorApplication.update += MoveNext;
        }


        internal void MoveNext()
        {
            if (owner != null && !owner.IsAlive)
            {
                EditorApplication.update -= MoveNext;
                return;
            }

            bool done = ProcessIEnumeratorRecursive(routine);
            isDone = !done;

            if (isDone)
            {
                EditorApplication.update -= MoveNext;
            }
        }

        private static Stack<IEnumerator> kIEnumeratorProcessingStack = new Stack<IEnumerator>(32);

        private bool ProcessIEnumeratorRecursive(IEnumerator enumerator)
        {
            var root = enumerator;
            while (enumerator.Current as IEnumerator != null)
            {
                kIEnumeratorProcessingStack.Push(enumerator);
                enumerator = enumerator.Current as IEnumerator;
            }

            processor.Set(enumerator.Current);
            var result = processor.MoveNext(enumerator);

            while (kIEnumeratorProcessingStack.Count > 1)
            {
                if (!result)
                {
                    result = kIEnumeratorProcessingStack.Pop().MoveNext();
                }
                else
                {
                    kIEnumeratorProcessingStack.Clear();
                }
            }

            if (kIEnumeratorProcessingStack.Count > 0 && !result && root == kIEnumeratorProcessingStack.Pop())
            {
                result = root.MoveNext();
            }

            return result;
        }

        internal void Stop()
        {
            owner = null;
            routine = null;
            EditorApplication.update -= MoveNext;
        }
    }
}