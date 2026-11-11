import { SortDirection as ExternalSortDirection } from 'react-data-grid';

import { SortDirection } from '@/constants/sort';

export function convertToDataGridSortDirection(
	direction: SortDirection
): ExternalSortDirection {
	switch (direction) {
		case SortDirection.ASC:
			return 'ASC';
		case SortDirection.DESC:
			return 'DESC';
		default:
			return 'ASC';
	}
}

export function convertToNativeSortDirection(
	direction: ExternalSortDirection
): SortDirection {
	switch (direction) {
		case 'ASC':
			return SortDirection.ASC;
		case 'DESC':
			return SortDirection.DESC;
		default:
			return SortDirection.ASC;
	}
}
