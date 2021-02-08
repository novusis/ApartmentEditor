using System;
using System.Collections.Generic;

public class ApartmentModel
{
    public event Action Change;
    private ApartmentConfig _config;
    private List<WallModel> _walls = new List<WallModel>();
    public List<WallModel> Walls => _walls;

    public ApartmentModel(ApartmentConfig apartmentConfig)
    {
        _config = apartmentConfig;

        foreach (var wallConfig in _config.Walls)
        {
            _walls.Add(new WallModel(wallConfig, _config));
        }

        Change?.Invoke();
    }
}