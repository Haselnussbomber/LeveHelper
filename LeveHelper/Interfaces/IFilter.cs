namespace LeveHelper.Interfaces;

public interface IFilter
{
    int Order { get; }
    FilterManager? FilterManager { get; set; }
    void Reload();
    void Reset();
    void Draw();
    bool Run();
    bool HasValue();
}
