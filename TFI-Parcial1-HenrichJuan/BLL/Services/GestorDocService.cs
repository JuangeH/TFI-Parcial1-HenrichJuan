using BLL.Contracts;
using DAL.Contracts;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class GestorDocService : IGestorDocService
    {
        private readonly IGestorDocRepo _gestorDocRepo;
        public GestorDocService(IGestorDocRepo gestorDocRepo)
        {
            _gestorDocRepo = gestorDocRepo;
        }

        public FileDataModel ValidarDoc(FileDataModel fileDataModel) => _gestorDocRepo.ListarByNom(fileDataModel);

        
    }
}
