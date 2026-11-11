import { createContext } from 'react';

import { TimesheetParametersContextValues } from '../types';

export const TimesheetParametersContext =
	createContext<TimesheetParametersContextValues>(
		{} as unknown as TimesheetParametersContextValues
	);
