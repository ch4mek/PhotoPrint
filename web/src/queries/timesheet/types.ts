import { UseMutateFunction } from '@tanstack/react-query';
import { AxiosError } from 'axios';

import { ByPageResponse, ServerResponseError } from '../types';

import { Roles } from '@/constants/roles';
import { BillableTimesheet } from '@/pages/TimeSheet/components/TimesheetEngineer/types';
import { TimesheetStatusType } from '@/pages/TimeSheet/types';

export interface DeleteTimesheetVariables {
	id: number | string;
}

export interface TimesheetManagerDataById {
	id: number;
	projectName: string;
	taskName: string;
	ftm: number;
	btm: number;
	reportForDate: string;
	projectId: number;
}

export interface TimesheetManagerDataByIdResponse {
	pageableListOfItems: {
		page: number;
		pageSize: number;
		total: number;
		items: Array<TimesheetManagerDataById>;
	};
}

type TimesheetManagerDataTimeSheetItem = Omit<
	TimesheetManagerDataById,
	'projectName' | 'taskName' | 'projectId'
>;

export interface TimesheetManagerData {
	userId: number;
	color: string;
	userName: string;
	totalTime: number;
	reportForDate: string;
	isActive: boolean;
	userDepartmentId: string;
	timesheets: Array<TimesheetManagerDataTimeSheetItem>;
	status: TimesheetStatusType;
	role: Roles;
}

export interface TimesheetManagerDataResponse {
	usersTimesheets: {
		page: number;
		pageSize: number;
		total: number;
		items: TimesheetManagerData[];
	};
}

export interface TimesheetData {
	id: number;
	projectName: string;
	taskName: string;
	ftm: number;
	btm: number | null;
	reportForDate: string;
	projectId: number;
	isProjectActive: boolean;

	clientId?: number;
	clientName?: string;
	projectColor?: string;
	tasksCount?: number;
	totalTrackedInMinutes?: number;
	userColor?: string;
}

export interface TimesheetUpsertResponse {
	id: number;
	createdById: number;
	createdAtUtc: string;
	taskName: string;
	ftm: number;
	btm: number;
	reportForDate: string;
	userId: number;
	projectId: number;
}

export interface TimesheetDeleteResponse {
	id: number;
	projectId: number;
	projectName: string;
	taskName: string;
	reportForDate: string;
	ftm: number;
	btm: number;
	userId: number;
	userName: string;
	color: string;
}

export interface UpdateTaskNameVariables {
	userId: number;
	projectId: number;
	oldTaskName: string;
	newTaskName: string;
	startDate: Date;
	endDate: Date;
}

export interface TimesheetWithBlockedDays
	extends ByPageResponse<TimesheetData> {
	blockedDays: string[];
}

interface ProjectWithBlockedDays {
	project: {
		id: number;
		name: string;
		clientName: string;
		isBillable: boolean;
		isCurrentUserAbleToSeeBillableTrackedTime: boolean;
		color: string;
	};
	disabledDates: [];
	billabilityDays: BillabilityDays[];
}

export interface BillabilityDays {
	isBillable: boolean;
	date: string;
}

export type ProjectsWithBlockedDaysResponse = Array<ProjectWithBlockedDays>;

export enum TrackedType {
	Billable = 'billable',
	Fact = 'fact',
}

export type MutateBillableUpsertTimesheet = UseMutateFunction<
	TimesheetUpsertResponse,
	AxiosError<ServerResponseError>,
	BillableTimesheet
>;

interface TimesheetCalendarTask {
	id: number;
	taskName: string;
	projectName: string;
	factTrackedTime: number;
}

export interface TimesheetCalendarResponse {
	date: string;
	factTrackedSum: number;
	tasks: TimesheetCalendarTask[];
}
