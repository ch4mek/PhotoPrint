import { SortDirection } from '@/constants/sort';

export const getIsAscendingFlagForSorting = (
	field: string,
	orderBy: string,
	order: string
) =>
	orderBy !== field ? null : orderBy === field && order === SortDirection.ASC;
