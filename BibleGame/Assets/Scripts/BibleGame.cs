using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;

namespace BibleGame
{
    namespace UI
    {
        public static class BibleUI
        {
            public static UIController iController;

            public static void Init(UIController controller)
            {
                iController = controller;
            }
        }
    }

    public static class Actions
    {
        public static Action<CanvasType> ChangePanelActions;
    }
}
