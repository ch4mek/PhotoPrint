import {
	areFromToCompatibleObjectsEqual,
	areArraysEqualByPrimitiveItems,
} from '@/helpers/areValuesEqual';
import { ProjectExitsGridRow } from '@/types/projectExits';

import {
	CLOSING_REASON_VALUE_NAME,
	DEPARTMENT_FILTER_VALUE_NAME,
	GROUP_FILTER_VALUE_NAME,
	initialProjectExistsFilters,
	projectExitGridColumns,
} from './constants';
import { ProjectExitsFilters } from './types';

export const compareProjectExitsFilters = ({
	idFilter,
	userFilter,
	departmentFilter,
	groupFilter,
	clientFilter,
	projectFilter,
	closingDateFilter,
	closingReasonFilter,
	statusFilter,
}: ProjectExitsFilters) => ({
	idFilter: idFilter === initialProjectExistsFilters.idFilter,
	userFilter: userFilter === initialProjectExistsFilters.userFilter,
	departmentFilter: areArraysEqualByPrimitiveItems(
		departmentFilter,
		initialProjectExistsFilters.departmentFilter
	),
	groupFilter: groupFilter === initialProjectExistsFilters.groupFilter,
	clientFilter: clientFilter === initialProjectExistsFilters.clientFilter,
	projectFilter: projectFilter === initialProjectExistsFilters.projectFilter,
	closingReasonFilter:
		closingReasonFilter === initialProjectExistsFilters.closingReasonFilter,
	closingDateFilter: areFromToCompatibleObjectsEqual(
		closingDateFilter,
		initialProjectExistsFilters.closingDateFilter
	),
	statusFilter: areArraysEqualByPrimitiveItems(
		statusFilter,
		initialProjectExistsFilters.statusFilter
	),
});

export const getRowKey = (row: ProjectExitsGridRow) => row.id;

export const convertOrderByQueryParam = (orderBy: string) => {
	switch (orderBy) {
		case projectExitGridColumns.department.key:
			return DEPARTMENT_FILTER_VALUE_NAME;
		case projectExitGridColumns.group.key:
			return GROUP_FILTER_VALUE_NAME;
		case projectExitGridColumns.closingReason.key:
			return CLOSING_REASON_VALUE_NAME;
		default:
			return orderBy;
	}
};
