import { Resources } from 'i18next';

import { ALL_ID } from '@/constants/filters';

export interface TimesheetParametersContextValues {
	period: Date[];
	clientFilter: { id: number | string; name: string } | undefined;
	projectFilter: { id: number | string; name: string } | undefined;
	isProjectsModeOn: boolean | undefined;
	isPersonalModeOn: boolean | undefined;
}

export interface InitialFilterValue {
	id: typeof ALL_ID;
	name: string;
}

export interface TimesheetModeFilterOption {
	id: string;
	fullName: string;
}

export enum TimesheetStatusType {
	UnderReview = 'UnderReview',
	Approved = 'Approved',
	Rejected = 'Rejected',
}

export interface TimesheetHeaderWrapperProps {
	shouldGetTimesheetStatus: boolean;
	isMonthFilter: boolean;
	isEmployeeOrSysAdminRole: boolean;
}

export type TimesheetStatusLabelKey = keyof Resources['timesheet']['status'];
export type TimesheetModeLabelKey = keyof Resources['timesheet']['modes'];
