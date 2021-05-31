public class DelegateStateManager
{
	public DelegateState currentState;

	public void ChangeState(DelegateState newState)
	{
		if (currentState != null)
		{
			currentState.active = false;
			currentState.Exit?.Invoke();
		}
		
		if (newState != null)
		{
			newState.active = true;
			newState.Enter?.Invoke();
			currentState = newState;
		}
	}

	public void UpdateCurrentState()
	{
		currentState?.Update?.Invoke();
	}
}