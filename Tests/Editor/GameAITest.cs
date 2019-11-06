using NUnit.Framework;
using System;
using Activ.GOAP.Test;
using S = Activ.GOAP.PlanningState;

namespace Activ.GOAP{
public class GameAITest : TestBase{

    AI x;

    [SetUp] public void Setup(){
        #if UNITY_2018_1_OR_NEWER
        x = new UnityEngine.GameObject().AddComponent<AI>();
        #else
        x = new AI();
        #endif
        x.config.frameBudget = 5;
    }

    // Use next goal from list if first one does not work
    [Test] public void Fallback(){
        Goal<Model> g1, g2;
        x.goals = new Goal<Model>[]{
            g1 = (s => false, null),
            g2 = (s => true,  null)
        };
        x.Update();
        o( x.status, S.Failed );
        o( x.goal, g1 );
        x.Update();
        o( x.status, S.Done );
        o( x.goal, g2 );
    }

    [Test] public void NoFallback(){
        Goal<Model> g1, g2;
        x.goals = new Goal<Model>[]{
            g1 = (s => true,  null),
            g2 = (s => false, null)
        };
        x.Update();
        o( x.status, S.Done );
        o( x.goal, g1 );
    }

}}
