using System.ComponentModel.DataAnnotations;

namespace Products.API.Controllers;

public class PagedQueryParameters
{
    [Range(1, int.MaxValue, ErrorMessage = "pageNumber debe ser mayor o igual a 1.")]
    public int PageNumber { get; set; } = 1;

    [Range(1, 100, ErrorMessage = "pageSize debe estar entre 1 y 100.")]
    public int PageSize { get; set; } = 10;
}