﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code.FightScene
{
    public class RoundStart : IFightBase
    {
        FightManager manager;
        Image fade;

        public RoundStart(FightManager managerRef)
        {
            manager = managerRef;

            foreach (Image img in manager.uiManager.elements)
            {
                if (img.name == "FadeBlack")
                    fade = img;
            }

            fade.GetComponent<Animator>().Play("FadeFromBlack", 0, 0);
        }

        public void StateStart()
        {

        }

        public void StateUpdate()
        {

        }
    }
}