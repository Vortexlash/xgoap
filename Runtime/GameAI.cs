using System;
#if UNITY_2018_1_OR_NEWER
using UnityEngine;
#endif

/*
How-to:
1 - Implement `Goal()` and `Model()`
2 - Each action returned by the solver should match
a public, no-arg function call supported by at least
one component.
3 - Override IsActing() (should be true while doing things, false
when available for planning)
*/
namespace Activ.GOAP{
// NOTE: derives from MonoBehaviour when used with Unity3D
// (see Runtime/Unity/GameAI.cs)
public abstract partial class GameAI<T> : SolverOwner{

    public bool          verbose;
    public Solver<T>     solver;
    public SolverParams  config   = new SolverParams();
    public Handlers      policies = new Handlers();
    public ActionHandler handler  = new ActionMap();

    public SolverStats   stats => solver;

    // Decide a goal and (optionally) a heuristic
    public abstract Goal<T> Goal();

    // Instantiates or updates the model using relevant world states
    public abstract T Model();

    // Override if you have a behavior while idle
    virtual public void Idle(){ }

    // This method should return false to indicate that the actor
    // is available for planning, true while the actor is effecting
    // game a-ctions
    virtual public bool IsActing() => false;

    public virtual void Update(){
        if(IsActing()) return;
        var model = Model();
        if(model == null) return;
        solver = solver ?? new Solver<T>();
        solver.maxNodes = config.maxNodes;
        solver.maxIter  = config.maxIter;
        if(handler is ActionMap m) m.verbose = verbose;
        handler.Effect( solver.isRunning
            ? solver.Iterate(config.frameBudget)?.Head()
            : solver.Next(model, Goal(), config.frameBudget)?.Head(),
            this);
        if(policies.OnResult(solver.state, ObjectName())){
            //ebug.Log($"Clear solver - {solver.state}");
            solver = null;
        }
    }

    protected virtual string ObjectName(){
        #if UNITY_2018_1_OR_NEWER
        return this.gameObject.name;
        #else
        return this.GetType().Name;
        #endif
    }

}}
