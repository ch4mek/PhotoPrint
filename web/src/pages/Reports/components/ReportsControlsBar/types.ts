import { Dispatch, SetStateAction } from 'react';

import { ReportsQueryParams } from '../../types';

import { DepartmentType } from '@/types/configuration';
import { Filter, WithMobileView } from '@/types/filter';
import { TimePeriod, TrackedTimeType } from '@/types/index';
import { NavigatorDate } from '@/types/report';

interface EditDate {
	year: number;
	month?: string;
	quarter?: string;
}

export interface ReportControlsProps extends WithMobileView {
	queryParams: ReportsQueryParams;
	setQueryParams: Dispatch<SetStateAction<ReportsQueryParams>>;
	setDate: (value: NavigatorDate | any) => void;
	showToast: () => void;

	userFilter: Filter<string | number>;
	clientFilter: Filter<string | number>;
	projectFilter: Filter<string | number>;
	departmentFilter: DepartmentType;
	groupFilter: Filter<string | number>;
	hoursFilter: {
		id: TrackedTimeType;
		name: string;
	};

	editDate: EditDate;
	timePeriod: TimePeriod;
	handleSetTimePeriod: (newValue: TimePeriod) => void;
	handleSetEditDate: (newValue: EditDate) => void;
	handleClientsFilterChange: (newValue: Filter<string | number> | null) => void;
	handleProjectsFilterChange: (
		newValue: Filter<string | number> | null
	) => void;
	handleUserFilterChange: (newValue: Filter<string | number> | null) => void;
	handleGroupFilterChange: (newValue: Filter<string | number> | null) => void;
	handleDepartmentFilterChange: (newValue: DepartmentType | null) => void;
	handleHoursFilterChange: (newValue: Filter<TrackedTimeType> | null) => void;

	handleResetFilters: (() => void) | undefined;
	isResetDisabled: boolean;

	handleClosePopover?: () => void;
	refetch: VoidFunction;
	isReportsFilterChanged: boolean;
	handleFiltersStatusReset: VoidFunction;
}
