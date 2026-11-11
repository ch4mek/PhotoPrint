import { Dispatch, SetStateAction } from 'react';

import { ReportsQueryParams } from '../../types';

import { Roles } from '@/constants/roles';
import { DepartmentType } from '@/types/configuration';
import { Filter } from '@/types/filter';
import { TrackedTimeType } from '@/types/index';
import { NavigatorDate } from '@/types/report';

export interface ReportControlsProps {
	date: {
		year: number;
		month?: string;
		quarter?: string;
	};
	queryParams: ReportsQueryParams;
	setQueryParams: Dispatch<SetStateAction<ReportsQueryParams>>;
	setDate: (value: NavigatorDate | any) => void;
	showToast: () => void;
	role: Roles;
	refetch: VoidFunction;
}

export interface GenerateNewReportsSearchParamsProps {
	projectFilter: Filter<string | number>;
	clientFilter: Filter<string | number>;
	departmentFilter: DepartmentType;
	userFilter: Filter<string | number>;
	groupFilter: Filter<string | number>;
	hoursFilter: { id: TrackedTimeType; name: string };
	editDateFilter: {
		year: number;
		month?: string | undefined;
		quarter?: string | undefined;
	};
}

export interface UseReportsFiltersProps {
	showToast: () => void;
	setQueryParams: Dispatch<SetStateAction<ReportsQueryParams>>;
	date: {
		year: number;
		month?: string;
		quarter?: string;
	};
	setDate: (value: NavigatorDate | any) => void;
	role: Roles;
}
