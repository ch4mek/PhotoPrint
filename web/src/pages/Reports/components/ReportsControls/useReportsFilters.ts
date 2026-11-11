import { useCallback, useMemo, useState } from 'react';

import { getInitialHoursFilter } from '../utils';

import {
	initialClientsValue,
	initialProjectsValue,
	initialUserFilterValue,
	initialDepartmentsFilterValue,
	initialGroupFilterValue,
} from '@/constants/filters';
import {
	RP_CLIENT_FILTER_STORAGE_KEY,
	RP_DATE_FILTER_STORAGE_KEY,
	RP_DATE_SELECTOR_FILTER_STORAGE_KEY,
	RP_DEPARTMENT_FILTER_STORAGE_KEY,
	RP_GROUP_FILTER_STORAGE_KEY,
	RP_HOURS_FILTER_STORAGE_KEY,
	RP_PROJECT_FILTER_STORAGE_KEY,
	RP_USER_FILTER_STORAGE_KEY,
} from '@/constants/storageKeys';
import { generateYearsCompanyExistence } from '@/helpers/date';
import { generateInitialData } from '@/helpers/generateInitialData';
import { useStickyState } from '@/hooks/useStickyState';
import { InitialFilterValue } from '@/pages/TimeSheet/types';
import { DepartmentType } from '@/types/configuration';
import type { Filter } from '@/types/filter';
import { TimePeriod, TrackedTimeType } from '@/types/index';

import { UseReportsFiltersProps } from './types';

const initialData = generateInitialData(TimePeriod.Month);

export const useReportsFilters = ({
	showToast,
	setQueryParams,
	date,
	setDate,
	role,
}: UseReportsFiltersProps) => {
	const [isReportsFilterChanged, setIsReportsFilterChanged] =
		useState<boolean>(false);

	const [clientFilter, setClientFilter] = useStickyState<
		Filter | InitialFilterValue
	>(initialClientsValue, RP_CLIENT_FILTER_STORAGE_KEY);

	const [projectFilter, setProjectFilter] = useStickyState<
		Filter | InitialFilterValue
	>(initialProjectsValue, RP_PROJECT_FILTER_STORAGE_KEY);

	const [userFilter, setUserFilter] = useStickyState<
		Filter | InitialFilterValue
	>(initialUserFilterValue, RP_USER_FILTER_STORAGE_KEY);

	const [editDate, setEditDate] = useStickyState(
		date,
		RP_DATE_FILTER_STORAGE_KEY
	);
	const [timePeriod, setTimePeriod] = useStickyState(
		TimePeriod.Month,
		RP_DATE_SELECTOR_FILTER_STORAGE_KEY
	);

	const [departmentFilter, setDepartmentFilter] =
		useStickyState<DepartmentType>(
			initialDepartmentsFilterValue,
			RP_DEPARTMENT_FILTER_STORAGE_KEY
		);

	const [hoursFilter, setHoursFilter] = useStickyState(
		getInitialHoursFilter(role),
		RP_HOURS_FILTER_STORAGE_KEY
	);

	const [groupFilter, setGroupFilter] = useStickyState<
		Filter | InitialFilterValue
	>(initialGroupFilterValue, RP_GROUP_FILTER_STORAGE_KEY);

	const handleFiltersStatusReset = useCallback(() => {
		if (isReportsFilterChanged) {
			setIsReportsFilterChanged(false);
		}
	}, [isReportsFilterChanged]);

	const handleFiltersStatusUpdate = useCallback(() => {
		if (!isReportsFilterChanged) {
			setIsReportsFilterChanged(true);
		}
	}, [isReportsFilterChanged]);

	const handleSetTimePeriod = useCallback((newValue: TimePeriod) => {
		setTimePeriod(newValue);
		handleFiltersStatusUpdate();
	}, []);

	const handleSetEditDate = useCallback(
		(newValue: { year: number; month?: string; quarter?: string }) => {
			setEditDate(newValue);
			handleFiltersStatusUpdate();
		},
		[]
	);

	const handleHoursFilterChange = useCallback(
		(newHoursFilterValue: Filter<TrackedTimeType> | null) => {
			showToast();
			handleFiltersStatusUpdate();

			if (!newHoursFilterValue) {
				setHoursFilter(getInitialHoursFilter(role));

				return;
			}

			setHoursFilter(newHoursFilterValue);
		},
		[]
	);

	const handleClientsFilterChange = useCallback((newValue: Filter | null) => {
		showToast();
		handleFiltersStatusUpdate();

		if (newValue) {
			setClientFilter(newValue);
		} else {
			setClientFilter(initialClientsValue);
		}

		setUserFilter(initialUserFilterValue);
		setProjectFilter(initialProjectsValue);
	}, []);

	const handleProjectsFilterChange = useCallback((newValue: Filter | null) => {
		showToast();
		handleFiltersStatusUpdate();

		if (newValue) {
			setProjectFilter(newValue);
		} else {
			setProjectFilter(initialProjectsValue);
		}

		setUserFilter(initialUserFilterValue);
	}, []);

	const handleDepartmentFilterChange = useCallback(
		(newValue: DepartmentType | null) => {
			showToast();
			handleFiltersStatusUpdate();

			if (!newValue) {
				setDepartmentFilter(initialDepartmentsFilterValue);

				return;
			}

			setDepartmentFilter(newValue);
		},
		[]
	);

	const handleUserFilterChange = useCallback((newValue: Filter | null) => {
		showToast();
		handleFiltersStatusUpdate();

		if (!newValue) {
			setUserFilter(initialUserFilterValue);

			return;
		}
		setUserFilter(newValue);
	}, []);

	const handleGroupFilterChange = useCallback((newValue: Filter | null) => {
		showToast();
		handleFiltersStatusUpdate();

		if (!newValue) {
			setGroupFilter(initialGroupFilterValue);

			return;
		}
		setGroupFilter(newValue);
	}, []);

	const resetFilters = useCallback(() => {
		setHoursFilter(getInitialHoursFilter(role));
		setClientFilter(initialClientsValue);
		setProjectFilter(initialProjectsValue);
		setDepartmentFilter(initialDepartmentsFilterValue);
		setUserFilter(initialUserFilterValue);
		setGroupFilter(initialGroupFilterValue);
		setDate(initialData);
		setEditDate(initialData);
		setTimePeriod(TimePeriod.Month);
		setQueryParams({
			monthOfYear: [initialData.month] as string[],
			year: generateYearsCompanyExistence()[0],
			trackedTimeType: getInitialHoursFilter(role).id,
		});
	}, []);

	const isResetDisabled = useMemo(
		() =>
			clientFilter.id === initialClientsValue.id &&
			projectFilter.id === initialProjectsValue.id &&
			departmentFilter.id === initialDepartmentsFilterValue.id &&
			userFilter.id === initialUserFilterValue.id &&
			hoursFilter.id === getInitialHoursFilter(role).id &&
			initialData.year === editDate.year &&
			initialData.month === editDate.month &&
			timePeriod === TimePeriod.Month &&
			initialData.quarter === editDate.quarter &&
			groupFilter.id === initialGroupFilterValue.id,
		[
			clientFilter,
			projectFilter,
			userFilter,
			editDate,
			departmentFilter,
			hoursFilter,
			initialData,
			timePeriod,
			groupFilter,
		]
	);

	const handleResetFilters = useMemo(
		() => (!isResetDisabled ? resetFilters : undefined),
		[isResetDisabled]
	);

	return {
		isReportsFilterChanged,
		clientFilter,
		projectFilter,
		userFilter,
		departmentFilter,
		editDate,
		timePeriod,
		groupFilter,
		handleSetTimePeriod,
		handleSetEditDate,
		handleClientsFilterChange,
		handleProjectsFilterChange,
		handleUserFilterChange,
		handleDepartmentFilterChange,
		handleFiltersStatusReset,
		hoursFilter,
		handleHoursFilterChange,
		handleGroupFilterChange,
		handleResetFilters,
		isResetDisabled,
	};
};
