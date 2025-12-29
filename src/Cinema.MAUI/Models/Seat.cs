using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Text.Json.Serialization;

namespace Cinema.MAUI.Models;

public class Seat : INotifyPropertyChanged
{
    // JSON properties
    [JsonPropertyName("id")]
    public required int Id { get; set; }
    [JsonPropertyName("row")]
    public required string Row { get; set; }
    [JsonPropertyName("seatNumber")]
    public required int SeatNumber { get; set; }
    [JsonPropertyName("isAvailable")]
    public required bool IsAvailable { get; set; }

    // Backing field and property
    private bool _isSelected;
    public bool IsSelected
    {
        get { return _isSelected; }
        set {
            if (_isSelected != value)
            {
                _isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
                OnPropertyChanged(nameof(SeatIcon));
            }
        }
    }

    public string SeatIcon
    {
        get
        {
            if (!IsAvailable)
            {
                return "seatunavailable_icon.png";
            }
            else if (IsSelected)
            {
                return "seatselected_icon.png";
            }

            return "seatavailable_icon.png";
        }
    }

    // INotifyPropertyChanged implementation
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
