namespace EliteKit.Vehicle.Blazor.Components.Pages;

public partial class Counter
{
    string Message = "默認值";

    int currentCount = 0;

    void IncrementCount()
    {
        currentCount++;
    }
}