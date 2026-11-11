import {
	createContext,
	PropsWithChildren,
	useCallback,
	useContext,
	useMemo,
	useState,
} from 'react';

import { INITIAL_SELECTED_DATE } from '../constants';
import { CalendarDate, CalendarFiltersContextValues } from '../types';

const CalendarFiltersContext = createContext<CalendarFiltersContextValues>(
	{} as CalendarFiltersContextValues
);

export function CalendarFiltersProvider({ children }: PropsWithChildren) {
	const [date, setDate] = useState<CalendarDate>(INITIAL_SELECTED_DATE);

	const [inquiryTemplateName, setInquiryTemplateName] = useState<string | null>(
		null
	);

	const [inquiryId, setInquiryId] = useState<string | null>(null);

	const resetFilters = useCallback(() => {
		setDate(INITIAL_SELECTED_DATE);
		setInquiryTemplateName(null);
		setInquiryId(null);
	}, []);

	const contextValue: CalendarFiltersContextValues = useMemo(
		() => ({
			date,
			setDate,
			inquiryId,
			inquiryTemplateName,
			setInquiryId,
			setInquiryTemplateName,
			resetFilters,
		}),
		[date, inquiryTemplateName, resetFilters, inquiryId]
	);

	return (
		<CalendarFiltersContext value={contextValue}>
			{children}
		</CalendarFiltersContext>
	);
}

export const useCalendarFilters = () => useContext(CalendarFiltersContext);
