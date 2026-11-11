import pickBy from 'lodash/pickBy';

import { getInitialTimePeriodFilter } from '../utils';

import {
	ALL_ID,
	BILLABLE_HOURS_FILTER,
	extraFiltersBy,
	FACT_HOURS_FILTER,
} from '@/constants/filters';
import { isRegularEmployee, Roles } from '@/constants/roles';
import {
	RP_DATE_FILTER_STORAGE_KEY,
	RP_CLIENT_FILTER_STORAGE_KEY,
	RP_DEPARTMENT_FILTER_STORAGE_KEY,
	RP_HOURS_FILTER_STORAGE_KEY,
	RP_PROJECT_FILTER_STORAGE_KEY,
	RP_USER_FILTER_STORAGE_KEY,
	RP_GROUP_FILTER_STORAGE_KEY,
} from '@/constants/storageKeys';
import { generateQuarter } from '@/helpers/date';
import { getResourceGroupFilterParam } from '@/helpers/getResourceGroupFilterParam';
import { minutesToHours } from '@/helpers/minutesToHours';
import { getItemFromStorage } from '@/helpers/storage';
import { GenerateNewReportsSearchParamsProps } from '@/pages/Reports/components/ReportsControls/types';
import { ReportsQueryParams } from '@/pages/Reports/types';

// TODO: typescript-refactor
export function sumDuration(
	arr: any[] | undefined,
	property?: string | undefined
) {
	return minutesToHours(
		arr?.reduce(
			(prev: unknown, value: Record<string, string>) =>
				prev + value[property || 'totalTimeInMinutes'],
			0
		)
	);
}

export const generateNewReportsSearchParams = ({
	projectFilter,
	clientFilter,
	departmentFilter,
	userFilter,
	hoursFilter,
	editDateFilter,
	groupFilter,
}: GenerateNewReportsSearchParamsProps) => {
	const { month, quarter, year } = editDateFilter;
	const { All } = extraFiltersBy;

	const newQueryParams = {
		projectId: projectFilter.id !== ALL_ID ? projectFilter.id : undefined,
		clientId: clientFilter.id !== ALL_ID ? clientFilter.id : undefined,
		groupId: getResourceGroupFilterParam(groupFilter),
		departmentId:
			departmentFilter.id !== All ? departmentFilter?.id : undefined,
		...(month && {
			monthOfYear: [month],
		}),
		...(quarter && {
			monthOfYear: generateQuarter(quarter),
		}),
		year,
		userId: userFilter.id !== All ? userFilter.id : undefined,
		trackedTimeType: hoursFilter.id,
	};

	return pickBy(
		newQueryParams,
		(value) => value !== undefined
	) as unknown as ReportsQueryParams;
};

export const getInitialHoursFilter = (role: Roles) =>
	isRegularEmployee(role) ? FACT_HOURS_FILTER : BILLABLE_HOURS_FILTER;

export const getInitialReportsSearchParams = (
	initialData: ReturnType<typeof getInitialTimePeriodFilter>,
	role: Roles,
	userId: number
) => {
	const userFilter = getItemFromStorage(RP_USER_FILTER_STORAGE_KEY, userId);
	const clientFilter = getItemFromStorage(RP_CLIENT_FILTER_STORAGE_KEY, userId);
	const projectFilter = getItemFromStorage(
		RP_PROJECT_FILTER_STORAGE_KEY,
		userId
	);
	const hoursFilter = getItemFromStorage(RP_HOURS_FILTER_STORAGE_KEY, userId);
	const groupFilter = getItemFromStorage(RP_GROUP_FILTER_STORAGE_KEY, userId);
	const departmentFilter = getItemFromStorage(
		RP_DEPARTMENT_FILTER_STORAGE_KEY,
		userId
	);
	const editDateFilter = getItemFromStorage(RP_DATE_FILTER_STORAGE_KEY, userId);

	const areAllFiltersPresent =
		userFilter &&
		clientFilter &&
		projectFilter &&
		hoursFilter &&
		groupFilter &&
		departmentFilter &&
		editDateFilter;

	if (areAllFiltersPresent) {
		return generateNewReportsSearchParams({
			clientFilter,
			departmentFilter,
			editDateFilter,
			hoursFilter,
			userFilter,
			projectFilter,
			groupFilter,
		});
	}

	return {
		monthOfYear: initialData.months,
		year: initialData.year,
		trackedTimeType: getInitialHoursFilter(role).id,
	};
};
