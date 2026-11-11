import { UseMutateFunction } from '@tanstack/react-query';
import { AxiosError } from 'axios';
import { createContext } from 'react';

import { UpdateTimesheetStatusProps } from '@/queries/timesheet/useUpdateUserTimesheetStatus';

interface MutateTimesheetStatusContextProps {
	mutate: UseMutateFunction<
		UpdateTimesheetStatusProps,
		AxiosError<unknown, any>,
		UpdateTimesheetStatusProps,
		unknown
	>;
}

export const MutateTimesheetStatusContext =
	createContext<MutateTimesheetStatusContextProps>(
		{} as unknown as MutateTimesheetStatusContextProps
	);
