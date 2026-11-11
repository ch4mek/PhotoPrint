import { useContext } from 'react';

import { CalendarOperationsContext } from '../context/CalendarOperationsContext';

export const useCalendarContextOperations = () =>
	useContext(CalendarOperationsContext);
