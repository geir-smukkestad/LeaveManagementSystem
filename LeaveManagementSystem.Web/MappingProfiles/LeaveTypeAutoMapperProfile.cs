using AutoMapper;
using LeaveManagementSystem.Web.Data;
using LeaveManagementSystem.Web.Models.LeaveTypes;

namespace LeaveManagementSystem.Web.MappingProfiles
{
    public class LeaveTypeAutoMapperProfile : Profile
    {
        public LeaveTypeAutoMapperProfile()
        {
#if true
            CreateMap<LeaveType, LeaveTypeReadOnlyVM>();
#else
            // Member names LeaveType.Days and IndexVM.NumberOfDays differ =>
            // We have to set up a manual mapping for it.
            CreateMap<LeaveType, IndexVM>()
                .ForMember(dest => dest.Days, opt => opt.MapFrom(src => src.NumberOfDays));
#endif
            CreateMap<LeaveTypeReadOnlyVM, LeaveType>();
            CreateMap<LeaveTypeCreateVM, LeaveType>();
            CreateMap<LeaveTypeEditVM, LeaveType>().ReverseMap(); // Both directions
        }
    }
}
