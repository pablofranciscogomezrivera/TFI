using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominio.Enums;

namespace Dominio.Entidades;

public class Usuario
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public TipoAutoridad TipoAutoridad { get; set; }
    public string User
    {
        get => Email;
        set => Email = value;
    }

    public string Password
    {
        get => PasswordHash;
        set => PasswordHash = value;
    }
}
