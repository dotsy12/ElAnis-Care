using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElAnis.Entities.DTO.Admin
	{
		public class PaginatedResult<T>
		{
			public List<T> Items { get; set; } = new List<T>();
			public int TotalCount { get; set; }
			public int Page { get; set; }
			public int PageSize { get; set; }
			public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
			public bool HasNextPage => Page < TotalPages;
			public bool HasPreviousPage => Page > 1;
		}
	}

