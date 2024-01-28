using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class Pedido
    {
        public Guid Id { get; set; }
        public Usuario Usuario { get; set; }
        public DateTime DataCriacao { get; set; }

        public Pedido(Guid id, Usuario usuario)
        {
            Id = id;
            Usuario = usuario;
        }

        public Pedido()
        { }


        public override string ToString()
            => $"Pedido Id: {Id}, Usuário? {Usuario.Nome}, Data de Criação: {DataCriacao:dd/MM/yyyy}";
    }
}
