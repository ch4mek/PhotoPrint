import { TrackedTimeType } from '../types';

import { DepartmentType } from '@/types/configuration';
import { ProjectsTableFilters } from '@/types/projectsTableFilters';

import { fromToDateObject } from './defaultValues';

export const ALL_ID = 'All' as const;

export const extraFiltersBy = {
	All: 'All',
	Personal: 'Personal',
};

export const filterByPeriod = {
	weekFilter: 'This Week',
	monthFilter: 'This Month',
};

export const initialProjectsValue = {
	id: ALL_ID,
	name: 'All Projects',
} as const;

export const initialClientsValue = {
	id: ALL_ID,
	name: 'All Clients',
} as const;

export const initialUserFilterValue = {
	id: ALL_ID,
	name: 'All Users',
};

export const initialGroupFilterValue = {
	id: ALL_ID,
	name: 'All Groups',
};

export const initialDepartmentsFilterValue: DepartmentType = {
	id: ALL_ID,
	shortName: 'All Departments',
	fullName: 'All Departments',
};

export const NO_VALUE_FILTER_OPTION = {
	description: 'N/A',
	id: 'N/A',
	name: 'N/A',
	fullName: 'N/A',
	shortName: 'N/A',
	location: null,
};

export const BILLABLE_HOURS_FILTER = {
	id: TrackedTimeType.Billable,
	name: 'Billable',
};

export const FACT_HOURS_FILTER = {
	id: TrackedTimeType.Fact,
	name: 'Fact',
};

export const HOURS_FILTER_OPTIONS = [BILLABLE_HOURS_FILTER, FACT_HOURS_FILTER];

export const DEFAULT_PROJECTS_FILTER_VALUES: ProjectsTableFilters = {
	projectName: '',
	clientName: '',
	isActive: [],
	billability: [],
	startDate: fromToDateObject,
	endDate: fromToDateObject,
};
