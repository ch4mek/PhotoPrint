import { lowercaseFirstSymbol } from '@/helpers/lowercaseFirstSymbol';

export const getStringFilterValues = (
	filters: Array<[{ key: string }, string | null]>
) =>
	filters
		.map(([column, filterValue]) => ({
			filterColumn: lowercaseFirstSymbol(column.key),
			filterValue,
		}))
		.filter(({ filterColumn, filterValue }) => filterColumn && filterValue);
