import { FromToProps } from '@/components/Grid';
import { SelectionModalOption } from '@/components/Grid/renderers/header/types';
import { AttendenceDirection } from '@/types/attendances';
import { SetFilters } from '@/types/helpers';

export interface AttendanceDetailedGridRow {
	id: string;
	userFullNameRu: string;
	userFullName: string;
	department: string;
	group: string;
	direction: AttendenceDirection;
	doorNumber: string;
	location: string;
	date: string;
	time: string;
	isUserActive: boolean;
}

export interface AttendanceDetailedFilters {
	id: string;
	user: string;
	departments: string[];
	group: string;
	direction: AttendenceDirection[];
	door: string;
	location: string;
	date: FromToProps<Date>;
	time: FromToProps<string>;
}

export interface AttendanceDetailedSetFilters
	extends SetFilters<AttendanceDetailedFilters> {}

export interface AttendanceDetailedOptions {
	departamentsOptions: SelectionModalOption[];
	userOptions: string[];
	groupOptions: string[];
}
