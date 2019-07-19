using System;
using CleverCrow.Fluid.Dialogues.Nodes;
using UnityEditor;
using UnityEngine;

namespace CleverCrow.Fluid.Dialogues.Editors.NodeDisplays {
    public abstract class NodeDisplayBase {
        private const float PADDING_HEADER = 5;
        private const float PADDING_CONTENT = 20;
        private const float HEADER_HEIGHT = 20;

        private NodeBoxStyle _contentStyle;
        private NodeBoxStyle _headerStyle;
        private NodeBoxStyle _containerHighlight;
        private HeaderTextStyle _headerTextStyle;
        private float _cachedContentHeight;
        private bool _dragInit;
        private bool _isDragging;

        protected DialogueWindow Window { get; private set; }
        protected virtual string NodeTitle => Data.name;
        protected SerializedObject serializedObject { get; private set; }
        public NodeDataBase Data { get; private set; }
        public bool IsSelected { get; private set; }

        protected virtual Color NodeColor => Color.gray;
        protected virtual float NodeWidth { get; } = 100;

        public bool IsMemoryLeak => Data == null;

        public void Setup (DialogueWindow window, NodeDataBase data) {
            Window = window;
            Data = data;

            var bodyColor = NodeColor;
            _contentStyle = new NodeBoxStyle(bodyColor, bodyColor);
            _containerHighlight = new NodeBoxStyle(Color.white, Color.clear);
            _headerTextStyle = new HeaderTextStyle();

            var headerColor = bodyColor * 1.3f;
            headerColor.a = 1f;
            _headerStyle = new NodeBoxStyle(headerColor, headerColor);

            Data.rect.width = NodeWidth;
            serializedObject = new SerializedObject(data);

            OnSetup();
        }

        protected virtual void OnSetup () {
        }

        public void Print () {
            PrintHeader();
            PrintBody();

            if (IsSelected) {
                GUI.Box(Data.rect, GUIContent.none, _containerHighlight.Style);
            }
        }

        private void PrintHeader () {
            var headerBox = new Rect(Data.rect.x, Data.rect.y, Data.rect.width, HEADER_HEIGHT);
            var headerArea = new Rect(
                headerBox.x + PADDING_HEADER / 2f,
                headerBox.y + PADDING_HEADER / 2f,
                headerBox.width - PADDING_HEADER,
                headerBox.height - PADDING_HEADER);

            GUI.Box(headerBox, GUIContent.none, _headerStyle.Style);
            GUI.Label(headerArea, NodeTitle, _headerTextStyle.Style);
        }

        private void PrintBody () {
            var box = new Rect(Data.rect.x, Data.rect.y + HEADER_HEIGHT, Data.rect.width, Data.rect.height - HEADER_HEIGHT);
            var content = new Rect(
                Data.rect.x + PADDING_CONTENT / 2f,
                Data.rect.y + HEADER_HEIGHT + PADDING_CONTENT / 2f,
                Data.rect.width - PADDING_CONTENT,
                Data.rect.height - PADDING_CONTENT);

            GUI.Box(box, GUIContent.none, _contentStyle.Style);

            GUILayout.BeginArea(content);

            OnPrintBody();
            if (Event.current.type == EventType.Repaint ) {
                var rect = GUILayoutUtility.GetLastRect();
                if (Math.Abs(rect.height - _cachedContentHeight) > 0.1f) {
                    Data.rect.height = rect.height + HEADER_HEIGHT + PADDING_CONTENT * 2;
                    _cachedContentHeight = rect.height;
                }
            }

            GUILayout.EndArea();
        }

        protected virtual void OnPrintBody () {
            GUILayout.Label(Data.name);
        }

        public void Select () {
            Selection.activeObject = Data;
            IsSelected = true;
        }

        public void Deselect () {
            IsSelected = false;
        }

        public void ProcessEvent (Event e) {
            switch (e.type) {
                case EventType.MouseDown when Data.rect.Contains(e.mousePosition):
                    Select();
                    _dragInit = false;
                    _isDragging = true;
                    GUI.changed = true;
                    break;
                case EventType.ContextClick when Data.rect.Contains(e.mousePosition):
                    Select();
                    ShowContextMenu();
                    break;
                case EventType.MouseDown:
                    if (IsSelected) {
                        Deselect();
                        GUI.changed = true;
                    }
                    break;
                case EventType.MouseDrag when _isDragging:
                    if (!_dragInit) {
                        Undo.RegisterCompleteObjectUndo(Data, "Move node");
                        _dragInit = true;
                    }

                    Data.rect.position += e.delta;
                    e.Use();
                    break;
                case EventType.MouseUp when IsSelected:
                    _isDragging = false;
                    break;
            }
        }

        public virtual void ShowContextMenu () {
        }
    }
}
