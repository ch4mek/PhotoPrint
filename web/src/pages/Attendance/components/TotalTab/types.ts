import { FromToProps } from '@/components/Grid';
import { SelectionModalOption } from '@/components/Grid/renderers/header/types';
import { SetFilters } from '@/types/helpers';

export interface AttendanceTotalGridRow {
	id: string;
	userId: string;
	user: string;
	group: string | null;
	department: string;
	date: string;
	time: string;
	totalTime: string;
	isUserActive: boolean;
}

export interface AttendanceTotalFilters {
	userId: string;
	user: string;
	departments: string[];
	group: string;
	date: FromToProps<Date>;
	time: FromToProps<string>;
}

export interface AttendanceTotalOptions {
	departamentsOptions: SelectionModalOption[];
	userOptions: string[];
	groupOptions: string[];
}

export interface AttendanceTotalSetFilters
	extends SetFilters<AttendanceTotalFilters> {}
