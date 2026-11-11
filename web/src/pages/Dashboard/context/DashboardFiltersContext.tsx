import {
	createContext,
	PropsWithChildren,
	useContext,
	useMemo,
	useState,
} from 'react';

import { DashboardFiltersContextValues } from '../types';

import { Departments } from '@/constants/departments';

const DashboardFiltersContext = createContext<DashboardFiltersContextValues>(
	{} as DashboardFiltersContextValues
);

export function DashboardFiltersProvider({ children }: PropsWithChildren) {
	const [selectedDepartmentId, setSelectedDepartmentId] =
		useState<Departments | null>(null);

	const contextValue: DashboardFiltersContextValues = useMemo(
		() => ({
			selectedDepartmentId,
			setSelectedDepartmentId,
		}),
		[selectedDepartmentId]
	);

	return (
		<DashboardFiltersContext value={contextValue}>
			{children}
		</DashboardFiltersContext>
	);
}

export const useDashboardFilters = () => useContext(DashboardFiltersContext);
