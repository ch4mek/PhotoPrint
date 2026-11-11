import { PageWrapper } from '@/pages/styles';

import { AbsenceCalendarGrid } from './components/AbsenceCalendarGrid';
import { CalendarFilters } from './components/CalendarFilters';
import { CalendarFiltersProvider } from './context/CalendarFiltersContext';

export function CalendarTab() {
	return (
		<PageWrapper>
			<CalendarFiltersProvider>
				<CalendarFilters />
				<AbsenceCalendarGrid />
			</CalendarFiltersProvider>
		</PageWrapper>
	);
}
