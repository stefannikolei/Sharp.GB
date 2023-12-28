namespace Sharp.GB.Controller;

public interface IController
{
    void SetButtonListener(IButtonListener listener);
}


public class NullController : IController
{
    public IController Instance => new NullController();
    
    public void SetButtonListener(IButtonListener listener)
    {
        // implementation goes here
    }
}
