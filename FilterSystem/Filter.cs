namespace LeveHelper;

public abstract class Filter
{
    protected FilterManager manager;

    public Filter(FilterManager manager)
    {
        this.manager = manager;
    }

    public abstract void Reset();
    public abstract void Draw();
    public abstract bool Run();
    public abstract void Set(dynamic value);

    protected FiltersState state
    {
        get
        {
            return this.manager.state;
        }
        set
        {
            this.manager.state = value;
        }
    }
}
