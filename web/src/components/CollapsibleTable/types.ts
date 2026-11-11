import { ReactNode } from 'react';

import { HomeManagerUsersResponse } from '@/types/home';

export interface UserRowInCollapsibleTable {
	color: string;
	countOfProjects: number;
	totalTrackedInMinutes: number;
	userId: number;
	userName: string;
}

export interface BaseCollapsibleTableProps {
	data?: HomeManagerUsersResponse;
	totalCount?: number;
	rowsPerPage: number;
	setRowsPerPage: (rowsPerPage: number) => void;
	page: number;
	setPage: (value: number) => void;
	children: ReactNode;
}

export interface GridTemplateColumnsForCollapsibleTableType {
	isTaskRow?: boolean;
	isDisplayFact?: boolean;
}
