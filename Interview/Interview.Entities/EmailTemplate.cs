using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interview.Entities
{

	public class EmailTemplate : EntityBase
	{

		public Guid ContestId { get; set; }
		public EmailTypes EmailType { get; set; }
		public string? EmailSubject { get; set; }
		public string? EmailBody { get; set; }
		public string? EmailCC { get; set; }

	}

}
