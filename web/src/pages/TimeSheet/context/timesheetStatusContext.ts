import { createContext } from 'react';

import { TimesheetModeFilterOption } from '../types';

import { Roles } from '@/constants/roles';

interface TimesheetStatusContextProps {
	role: Roles;
	departmentId: string;
	periodFilter: string;
	departmentTypeFilter: TimesheetModeFilterOption;
	month: number;
	year: number;
	handleUserClick: (id: number) => void;
	enableCellClick: (id: number) => boolean;
}

export const TimesheetStatusContext =
	createContext<TimesheetStatusContextProps>(
		{} as unknown as TimesheetStatusContextProps
	);
