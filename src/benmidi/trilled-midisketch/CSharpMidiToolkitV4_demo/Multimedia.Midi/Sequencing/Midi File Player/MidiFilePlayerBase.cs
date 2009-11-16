#region License

/* Copyright (c) 2006 Leslie Sanford
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy 
 * of this software and associated documentation files (the "Software"), to 
 * deal in the Software without restriction, including without limitation the 
 * rights to use, copy, modify, merge, publish, distribute, sublicense, and/or 
 * sell copies of the Software, and to permit persons to whom the Software is 
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in 
 * all copies or substantial portions of the Software. 
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN 
 * THE SOFTWARE.
 */

#endregion

#region Contact

/*
 * Leslie Sanford
 * Email: jabberdabber@hotmail.com
 */

#endregion

namespace Multimedia.Midi
{
    
    
    public abstract class MidiFilePlayerBase : StateMachineToolkit.StateMachine
    {
        
        protected const int StartID = 0;
        
        protected const int OpenID = 1;
        
        protected const int SetPositionID = 2;
        
        protected const int ContinueID = 3;
        
        protected const int StopID = 4;
        
        protected const int PlayingFinishedID = 5;
        
        protected const int BeatElapsedID = 6;
        
        private StateMachineToolkit.State stateStopped;
        
        private StateMachineToolkit.State statePlaying;
        
        private StateMachineToolkit.ActionHandler actionStart;
        
        private StateMachineToolkit.ActionHandler actionOpen;
        
        private StateMachineToolkit.ActionHandler actionSetPosition;
        
        private StateMachineToolkit.ActionHandler actionContinue;
        
        private StateMachineToolkit.ActionHandler actionStop;
        
        private StateMachineToolkit.ActionHandler actionRaisePlayingFinished;
        
        private StateMachineToolkit.ActionHandler actionRaiseBeatElapsed;
        
        public MidiFilePlayerBase()
        {
            this.Initialize();
        }
        
        private void Initialize()
        {
            this.InitializeStates();
            this.InitializeGuards();
            this.InitializeActions();
            this.InitializeTransitions();
            this.InitializeRelationships();
            this.InitializeHistoryTypes();
            this.InitializeInitialStates();
            this.Initialize(this.stateStopped);
        }
        
        private void InitializeStates()
        {
            StateMachineToolkit.EntryHandler enStopped = new StateMachineToolkit.EntryHandler(this.EntryStopped);
            StateMachineToolkit.ExitHandler exStopped = new StateMachineToolkit.ExitHandler(this.ExitStopped);
            this.stateStopped = new StateMachineToolkit.State(7, enStopped, exStopped);
            StateMachineToolkit.EntryHandler enPlaying = new StateMachineToolkit.EntryHandler(this.EntryPlaying);
            StateMachineToolkit.ExitHandler exPlaying = new StateMachineToolkit.ExitHandler(this.ExitPlaying);
            this.statePlaying = new StateMachineToolkit.State(7, enPlaying, exPlaying);
        }
        
        private void InitializeGuards()
        {
        }
        
        private void InitializeActions()
        {
            this.actionStart = new StateMachineToolkit.ActionHandler(this.Start);
            this.actionOpen = new StateMachineToolkit.ActionHandler(this.Open);
            this.actionSetPosition = new StateMachineToolkit.ActionHandler(this.SetPosition);
            this.actionContinue = new StateMachineToolkit.ActionHandler(this.Continue);
            this.actionStop = new StateMachineToolkit.ActionHandler(this.Stop);
            this.actionRaisePlayingFinished = new StateMachineToolkit.ActionHandler(this.RaisePlayingFinished);
            this.actionRaiseBeatElapsed = new StateMachineToolkit.ActionHandler(this.RaiseBeatElapsed);
        }
        
        private void InitializeTransitions()
        {
            StateMachineToolkit.Transition trans;
            trans = new StateMachineToolkit.Transition(null, null);
            trans.Actions.Add(this.actionStop);
            trans.Actions.Add(this.actionOpen);
            this.statePlaying.Transitions.Add(MidiFilePlayerBase.OpenID, trans);
            trans = new StateMachineToolkit.Transition(null, null);
            trans.Actions.Add(this.actionStop);
            trans.Actions.Add(this.actionSetPosition);
            trans.Actions.Add(this.actionContinue);
            this.statePlaying.Transitions.Add(MidiFilePlayerBase.SetPositionID, trans);
            trans = new StateMachineToolkit.Transition(null, this.stateStopped);
            trans.Actions.Add(this.actionStop);
            this.statePlaying.Transitions.Add(MidiFilePlayerBase.StopID, trans);
            trans = new StateMachineToolkit.Transition(null, this.stateStopped);
            trans.Actions.Add(this.actionStop);
            trans.Actions.Add(this.actionRaisePlayingFinished);
            this.statePlaying.Transitions.Add(MidiFilePlayerBase.PlayingFinishedID, trans);
            trans = new StateMachineToolkit.Transition(null, null);
            trans.Actions.Add(this.actionRaiseBeatElapsed);
            this.statePlaying.Transitions.Add(MidiFilePlayerBase.BeatElapsedID, trans);
            trans = new StateMachineToolkit.Transition(null, this.statePlaying);
            trans.Actions.Add(this.actionStart);
            this.stateStopped.Transitions.Add(MidiFilePlayerBase.StartID, trans);
            trans = new StateMachineToolkit.Transition(null, null);
            trans.Actions.Add(this.actionOpen);
            this.stateStopped.Transitions.Add(MidiFilePlayerBase.OpenID, trans);
            trans = new StateMachineToolkit.Transition(null, null);
            trans.Actions.Add(this.actionSetPosition);
            this.stateStopped.Transitions.Add(MidiFilePlayerBase.SetPositionID, trans);
            trans = new StateMachineToolkit.Transition(null, this.statePlaying);
            trans.Actions.Add(this.actionContinue);
            this.stateStopped.Transitions.Add(MidiFilePlayerBase.ContinueID, trans);
        }
        
        private void InitializeRelationships()
        {
        }
        
        private void InitializeHistoryTypes()
        {
            this.statePlaying.HistoryType = StateMachineToolkit.HistoryType.None;
            this.stateStopped.HistoryType = StateMachineToolkit.HistoryType.None;
        }
        
        private void InitializeInitialStates()
        {
        }
        
        protected virtual void EntryStopped()
        {
        }
        
        protected virtual void EntryPlaying()
        {
        }
        
        protected virtual void ExitStopped()
        {
        }
        
        protected virtual void ExitPlaying()
        {
        }
        
        protected abstract void Start(object[] args);
        
        protected abstract void Open(object[] args);
        
        protected abstract void SetPosition(object[] args);
        
        protected abstract void Continue(object[] args);
        
        protected abstract void Stop(object[] args);
        
        protected abstract void RaisePlayingFinished(object[] args);
        
        protected abstract void RaiseBeatElapsed(object[] args);
    }
}
