import { DependencyList } from 'react';

import { SortDirection } from '@/constants/sort';

export interface ShowModal<T> {
	isOpen: boolean;
	id?: T;
}

export interface UseTaableStateProps {
	initialRowsPerPage?: number;
	initialOrder?: SortDirection;
	initialOrderBy?: string;
	resetPageDeps?: DependencyList;
}
