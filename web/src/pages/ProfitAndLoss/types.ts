import { GridPaginationProps } from '@mms/mms-ui-library/components/Grids';
import { SortColumn } from 'react-data-grid';

import { SelectionModalOption } from '@/components/Grid/renderers/header/types';
import { SetFilters } from '@/types/helpers';

interface ProfitAndLossProjectDisplay {
	name: string;
	color: string | null;
}

export interface ProfitAndLossFilters {
	id: string;
	user: string;
	department: string[];
	project: string;
	client: string;
	legalEntity: string;
}

interface ProfitAndLossSetFilters extends SetFilters<ProfitAndLossFilters> {}

export enum ProfitAndLossFiltersActionType {
	SetIdFilter = 'SetIdFilter',
	SetUserFilter = 'SetUserFilter',
	SetDepartmentFilter = 'SetDepartmentFilter',
	SetProjectFilter = 'SetProjectFilter',
	SetClientFilter = 'SetClientFilter',
	SetLegalEntityFilter = 'SetLegalEntityFilter',
	Reset = 'Reset',
	Set = 'Set',
}

export type ProfitAndLossFiltersAction =
	| { type: ProfitAndLossFiltersActionType.SetIdFilter; payload: string }
	| { type: ProfitAndLossFiltersActionType.SetUserFilter; payload: string }
	| {
			type: ProfitAndLossFiltersActionType.SetDepartmentFilter;
			payload: string[];
	  }
	| { type: ProfitAndLossFiltersActionType.SetProjectFilter; payload: string }
	| { type: ProfitAndLossFiltersActionType.SetClientFilter; payload: string }
	| {
			type: ProfitAndLossFiltersActionType.SetLegalEntityFilter;
			payload: string;
	  }
	| { type: ProfitAndLossFiltersActionType.Set; payload: ProfitAndLossFilters }
	| { type: ProfitAndLossFiltersActionType.Reset };

export interface UseProfitAndLossColumnsProps {
	filters: ProfitAndLossFilters;
	setFilters: ProfitAndLossSetFilters;
	isResetFiltersDisabled: boolean;
	filterOptions: ProfitAndLossFilterOptions;
}

export interface ProfitAndLossFilterOptions {
	users: string[];
	departments: SelectionModalOption[];
}

export interface ProfitAndLossGridProps<GridRow> {
	rows: GridRow[];
	filters: ProfitAndLossFilters;
	setFilters: ProfitAndLossSetFilters;
	isResetFiltersDisabled: boolean;
	sortColumns: SortColumn[];
	filterOptions: ProfitAndLossFilterOptions;
	handleSortColumnChange: (sortColumns: SortColumn[]) => void;
	paginationProps?: GridPaginationProps;
	onToggleExpand: (id: string) => void;
}

export interface ProfitAndLossBaseGridRow {
	id: string;
	parentId: string | null;
	isParent: boolean;
	itemRowHeight: number;
	expanded: boolean;
	hasChildren: boolean;
	isActive: boolean;
	parentIndex: number;
	user: string;
	department: string;
	projects: ProfitAndLossProjectDisplay[];
	client: string;
	legalEntity: string;
	rate: number | null;
	currency: string | null;
	disabled?: boolean;
	isLastChild?: boolean;
}

export interface ProfitAndLossGridRowClass {
	isActive: boolean;
	isParent: boolean;
	expanded: boolean;
	isLastChild?: boolean;
	parentIndex: number;
}
