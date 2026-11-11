import {
	createContext,
	PropsWithChildren,
	useContext,
	useMemo,
	useState,
} from 'react';

import {
	initialAbsenceFilterValue,
	initialYearFilterValue,
} from '@/constants/absence';
import { AbsenceHistoryContextValues } from '@/context/Absence/types';

const AbsenceHistoryContext = createContext({} as AbsenceHistoryContextValues);

export function AbsenceHistoryProvider({ children }: PropsWithChildren) {
	const [yearFilter, setYearFilter] = useState(initialYearFilterValue);
	const [absenceFilter, setAbsenceFilter] = useState(initialAbsenceFilterValue);

	const discountContextValue = useMemo(
		() => ({
			yearFilter,
			setYearFilter,
			absenceFilter,
			setAbsenceFilter,
		}),
		[yearFilter, absenceFilter]
	);

	return (
		<AbsenceHistoryContext.Provider value={discountContextValue}>
			{children}
		</AbsenceHistoryContext.Provider>
	);
}

export const useAbsenceHistoryContext = () => useContext(AbsenceHistoryContext);
