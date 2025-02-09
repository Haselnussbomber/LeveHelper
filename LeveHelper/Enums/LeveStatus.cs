namespace LeveHelper.Enums;

[Flags]
public enum LeveStatus
{
    Any = 0,
    Incomplete = 1 << 1,
    Complete = 1 << 2,
    Accepted = 1 << 3,
    Started = 1 << 4,
    Failed = 1 << 5,
    ReadyForTurnIn = 1 << 6
}
