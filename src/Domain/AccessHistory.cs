using Domain.Common;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain;

public class AccessHistory : BaseEntity 
{
    public AccessHistory()
    {

    }
    
    public string VisitorName { get; set; } // Quem acessou o dado?
    public string Profile { get; set; } // Qual o cargo dele?

    public Guid? UserId { get; set; }
    [ForeignKey("UserId")]
    public virtual User User { get; set; } 

}