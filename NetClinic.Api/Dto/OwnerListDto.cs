namespace NetClinic.Api.Dto;

public class OwnerListDto
{
    public IEnumerable<OwnerDto> OwnerList { get; set; } = [];
    public int TotalPages { get; set; }
}