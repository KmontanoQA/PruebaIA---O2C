using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supermercado.Shared.Responses;

public class ActionResponse<T>  
{
    public bool WasSuccess { get; set; }
    public string? Message { get; set; }
    public T? Result { get; set; }
    public int? StatusCode { get; set; } // Código HTTP opcional para casos específicos (ej: 409 Conflict)
}

