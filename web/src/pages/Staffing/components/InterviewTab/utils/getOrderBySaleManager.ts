import { SortDirection } from 'react-data-grid';

export const getOrderBySaleManager = (sortDirection: SortDirection) =>
	`salesResponsibleUserId eq null ${sortDirection}, salesResponsibleUserFullNameRu ${
		sortDirection === 'ASC' ? 'eq' : 'ne'
	} null ${sortDirection}, salesResponsibleUserFullNameRu ${sortDirection}`;
