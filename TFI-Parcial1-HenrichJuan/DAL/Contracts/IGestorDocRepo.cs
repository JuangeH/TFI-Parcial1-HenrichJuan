using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Contracts
{
    public interface IGestorDocRepo
    {
        FileDataModel ListarByNom(FileDataModel fileDataModel);
    }
}
