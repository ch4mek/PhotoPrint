import { SortDirection } from '@/constants/sort';

export function invertSortDirection(direction: SortDirection) {
	if (direction === SortDirection.ASC) {
		return SortDirection.DESC;
	}

	return SortDirection.ASC;
}
