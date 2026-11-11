import { Column } from 'react-data-grid';

import {
	AbsenceCalendarFilterType,
	OptionsProps,
} from '@/pages/Requests/components/CalendarTab/types';
import { generateSetFilters } from '@/pages/Requests/components/ListTab/utils';
import { InquiryCalendarGroupRow } from '@/types/absences';
import { HandleRowClick } from '@/types/grid';

interface CalendarColumnConfig {
	width: number;
	fixed: boolean;
	key: string;
	name: string;
}

export interface CalendarColumns {
	userFullName: CalendarColumnConfig;
	department: CalendarColumnConfig;
}

export type UseAbsenceColumnsReturn = {
	columns: Array<Column<InquiryCalendarGroupRow>>;
	dateColumnWidth: number;
};

export interface UseAbsenceCalendarColumnsParams {
	period: Date[];
	gridOffsetWidth: number;
	absenceColumns: CalendarColumns;
	enableCellClick: (row: InquiryCalendarGroupRow) => boolean;
	onUserClick: HandleRowClick<InquiryCalendarGroupRow>;
	filters: AbsenceCalendarFilterType;
	setFilters: ReturnType<typeof generateSetFilters>;
	options: OptionsProps;
}
