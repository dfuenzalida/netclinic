namespace NetClinic.Api.Dto;

public class VetListDto
{
    public List<VetDto> VetList { get; set; } = new();
    public int TotalPages { get; set; } = 1;
}