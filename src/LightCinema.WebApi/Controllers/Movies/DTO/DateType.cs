using System.Runtime.Serialization;

namespace LightCinema.WebApi.Controllers.Movies.DTO;

public enum DateType
{
    [EnumMember(Value = "Today")]
    Today,
    
    [EnumMember(Value = "Tomorrow")]
    Tomorrow,
    
    [EnumMember(Value = "Sonn")]
    Soon
}