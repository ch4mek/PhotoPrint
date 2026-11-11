import { GridCellDefaultProps } from '@/components/Grid';

export type VacationCalendarCellProps = GridCellDefaultProps & {
	vacationDays?: number[];
	position?: string;
	name?: string;
};

export type VacationTypeWrapperProps = {
	backgroundColor: string;
	textColor: string;
};

export interface LeaveDay {
	date: string;
	type: string;
}

export interface EmployeeVacations {
	id: number;
	name: string;
	position: string;
	avatar: string;
	leaveDays: LeaveDay[];
}
