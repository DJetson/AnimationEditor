using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Ink;

namespace AnimationEditorCore.Utilities
{
    public static class InternalClipboard
    {
        private static StrokeCollection _Strokes = new StrokeCollection();

        public static bool HasData()
        {
            return _Strokes.Count > 0;
        }

        public static void SetData(StrokeCollection strokes)
        {
            _Strokes = strokes.Clone();
        }

        public static StrokeCollection GetData()
        {
            var data = _Strokes.Clone();

            return data;
        }

        public static void Clear()
        {
            _Strokes.Clear();
        }


    }
}
