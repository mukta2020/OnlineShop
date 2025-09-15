using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopOnline.Models.Dtos
{
    public class RefreshTokenRequestDto
    {
        public string Email { get; set; }
        public string RefreshToken { get; set; }
    }
}
