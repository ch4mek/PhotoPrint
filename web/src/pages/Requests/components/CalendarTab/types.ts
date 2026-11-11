import { SelectionModalOption } from '@/components/Grid/renderers/header/types';
import { months } from '@/constants/date';

export interface CalendarDate {
	month: number;
	year: number;
}

export interface CalendarFiltersContextValues {
	date: CalendarDate;
	setDate: (date: CalendarDate) => void;
	inquiryTemplateName: string | null;
	setInquiryTemplateName: (name: string | null) => void;
	inquiryId: string | null;
	setInquiryId: (id: string | null) => void;
	resetFilters: () => void;
}

export type AvailableMonths = (typeof months)[number];

export type MonthYearDate = `${AvailableMonths}, ${number}`;

export interface AbsenceCalendarFilterType {
	userFullName: string;
	userDepartment: string[];
}

export interface OptionsProps {
	users: string[];
	departments: SelectionModalOption[];
}
