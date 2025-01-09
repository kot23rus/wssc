using System.ComponentModel;

namespace WSSC.Client.Model;

public class UserData : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private string name = "";

    /// <summary>
    /// Имя юзера
    /// </summary>
    public string Name
    {
        get
        {
            return name;
        }
        set
        {
            if (value!=name)
            {
                name = value;
                PropertyChanged?.Invoke(this, new(nameof(Name)));
            }
        }
    }
}
