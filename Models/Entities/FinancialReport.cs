using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{
	public class FinancialReport
	{
		public int Year { get; set; }
		public int Month { get; set; }
		public decimal Revenue { get; set; }
		public decimal Cost { get; set; }
		public decimal Profit { get; set; }
	}

}
