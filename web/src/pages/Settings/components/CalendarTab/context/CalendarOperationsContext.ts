import { createContext } from 'react';

import { CalendarOperationsContextValues } from '../types';

export const CalendarOperationsContext = createContext(
	{} as CalendarOperationsContextValues
);
