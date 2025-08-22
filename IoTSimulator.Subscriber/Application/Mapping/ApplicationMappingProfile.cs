using AutoMapper;
using IoTSimulator.Subscriber.Domain.Models;
using IoTSimulator.Subscriber.Application.DTOs;

namespace IoTSimulator.Subscriber.Application.Mapping;

public class ApplicationMappingProfile : Profile
{
    public ApplicationMappingProfile()
    {
        // House mappings
        CreateMap<CreateHouseDto, House>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Rooms, opt => opt.Ignore());

        CreateMap<UpdateHouseDto, House>()
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Rooms, opt => opt.Ignore());

        CreateMap<House, HouseDto>();

        CreateMap<House, HouseWithRoomsDto>()
            .ForMember(dest => dest.Rooms, opt => opt.MapFrom(src => src.Rooms));

        // Room mappings
        CreateMap<CreateRoomDto, Room>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.House, opt => opt.Ignore())
            .ForMember(dest => dest.Devices, opt => opt.Ignore());

        CreateMap<UpdateRoomDto, Room>()
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.House, opt => opt.Ignore())
            .ForMember(dest => dest.Devices, opt => opt.Ignore());

        CreateMap<Room, RoomDto>();

        CreateMap<Room, RoomWithDevicesDto>()
            .ForMember(dest => dest.Devices, opt => opt.MapFrom(src => src.Devices));

        // IoTDevice mappings
        CreateMap<CreateIoTDeviceDto, IoTDevice>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Room, opt => opt.Ignore())
            .ForMember(dest => dest.SensorDataList, opt => opt.Ignore());

        CreateMap<UpdateIoTDeviceDto, IoTDevice>()
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Room, opt => opt.Ignore())
            .ForMember(dest => dest.SensorDataList, opt => opt.Ignore());

        CreateMap<IoTDevice, IoTDeviceDto>()
            .ForMember(dest => dest.Room, opt => opt.MapFrom(src => src.Room));

        // SensorData mappings
        CreateMap<CreateSensorDataDto, SensorData>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Device, opt => opt.Ignore());

        CreateMap<UpdateSensorDataDto, SensorData>()
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Device, opt => opt.Ignore());

        CreateMap<SensorData, SensorDataDto>();
    }
}
