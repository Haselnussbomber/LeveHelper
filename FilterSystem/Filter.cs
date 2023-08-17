namespace LeveHelper;

public abstract class Filter
{
    protected static readonly int InputWidth = 250;

    protected FilterManager manager;

    public Filter(FilterManager manager)
    {
        this.manager = manager;
    }

    public abstract void Reload();
    public abstract void Reset();
    public abstract void Draw();
    public abstract bool Run();
    public abstract bool HasValue();
    public abstract void Set(dynamic value);

    protected FiltersState state
    {
        get => manager.State;
        set => manager.State = value;
    }
}
