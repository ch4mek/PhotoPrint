import { SerializedStyles, Theme } from '@emotion/react';
import { MutateOptions } from '@tanstack/react-query';
import { CalculatedColumn, Column } from 'react-data-grid';

import { TimesheetStatusType } from '../../types';

import { ToastContextValues } from '@/context/types';
import {
	BillabilityDays,
	UpdateTaskNameVariables,
} from '@/queries/timesheet/types';

export interface TimeSheetsEngineerProps {
	isEngineer?: boolean;
	isActiveUser?: boolean;
	isDisabled?: boolean;
	disableMessage?: string;
	userId: number;
	overrideGridStyles?:
		| ((
				periodLenght: number,
				isMobileView: boolean,
				areColumnsFrozen: boolean
		  ) => (theme: Theme) => SerializedStyles)
		| null;
	isLoading?: boolean;
	onTotalTimeMapChange?: (
		newTimeMap: Map<string, TimeTrackItem | undefined>
	) => void;
	setParentRowTotalValue?: (
		dateStringDDMMYYYY: string,
		trackedInMinutes: number
	) => void;
	displayTotalRow?: boolean;
	onGetTotalTimeMapOnFirstRender?: (
		totalTimeMap: Map<string, TimeTrackItem | undefined>
	) => void;
	freezeTaskInfoColumn?: boolean;
	freezeTotalColumn?: boolean;
	displayTooltips?: boolean;
	areColumnsFrozen?: boolean;
	status?: TimesheetStatusType | null;
	onExpand?: (height: number) => void;
	hideHeaderRow?: boolean;
}

export interface TimeTrackItem {
	trackId: number;
	factTrackedInMinutes?: number;
	billableTrackedInMinutes?: number | null;
	reportForDate: string;
	isLoading: {
		fact?: boolean;
		billable?: boolean;
	};
}

export interface TimeSheetRow {
	rowId: string;
	index?: number;
	projectId: number;
	projectName: string;
	taskName: string;
	previousTaskName: string;
	timeMap: Map<string, TimeTrackItem | undefined>;
	displayTotalRow?: boolean;
	errorMessage: string | undefined;
	isValidTaskName: boolean;
	isTaskInfoEditable: boolean;
	areTimeCellsEditable: boolean;
	entireRowHoverMessage?: string;
	isProjectPrivate: boolean;
	isProjectBillable: boolean;
	isDoubleTimeCell: boolean;
	userHasAccessToProject: boolean;
	isProjectActive: boolean;
	isEditingForbiddenForPeriod: boolean;
	areTaskInfoFieldsTouched?: boolean;
	isMobileView: boolean;
	orderIndex?: number;
	disabledDates: string[];
	billabilityDays: BillabilityDays[];
}

export interface ProjectOption {
	id: number;
	client: string;
	name: string;
	type: string;
	isBillable: boolean;
	isCurrentUserAbleToSeeBillableTrackedTime: boolean;
	disabledDates: string[];
	billabilityDays: BillabilityDays[];
}

export interface TimeSheetContextValues {
	columns: Column<TimeSheetRow, unknown>[];
	projectOptions: ProjectOption[] | undefined;
	toast: ToastContextValues;
	isEngineer: boolean;
	userId: number | null;
	isActiveUser?: boolean;
	isPMOEmployee: boolean;
	isDisabledTimesheet?: boolean;
	disableMessage?: string;
	period: Date[];
	updateTotalTimeMap?: (
		dateStringDDMMYYYY: string,
		trackedInMinutes: number
	) => void;
	onTotalTimeMapChange?: (
		updatedTimeMap: Map<string, TimeTrackItem | undefined>
	) => void;
	onTaskNameUpdate: (
		props: Omit<UpdateTaskNameVariables, 'userId'>,
		options?:
			| MutateOptions<unknown, unknown, UpdateTaskNameVariables, unknown>
			| undefined
	) => void;
	onTaskTimeChange: (
		rowIdx: number,
		rowId: string,
		dayKey: string,
		newFactTime: number | null,
		newBillableTime: number | null,
		isLoading?: {
			fact?: boolean;
			billable?: boolean;
		}
	) => void;
	onParentRowChange: (
		rowIdx: number,
		rowId: string,
		newProjectInfo: Partial<TimeSheetRow>
	) => void;
	onAddNewRow: () => void;
	onUpdateTrackItemAfterSuccessRequest: (
		rowIdx: number,
		rowId: string,
		dayKey: string,
		timeTrackItem: TimeTrackItem | undefined
	) => void;
	onTaskInfoChange: (
		rowIdx: number,
		rowId: string,
		prevTaskName: string,
		prevProjectId: number,
		newTaskName: string,
		newProjectId: number,
		handleRowHasPrevTaskName: (row: TimeSheetRow) => TimeSheetRow
	) => void;
	displayTooltips: boolean;
	setTooltipContent: (
		tooltipContent: {
			anchorElement: HTMLDivElement;
			text?: string;
		} | null
	) => void;
	setIsCellLoading: (
		rowIdx: number,
		dayKey: string,
		isLoading: {
			fact?: boolean;
			billable?: boolean;
		}
	) => void;
	blockedDays: string[];
}

interface TimesheetRequest {
	taskName: string;
	reportForDate: string;
	projectId: number;
	userId: number | string | null;
}

export interface FactTimesheet extends Partial<TimesheetRequest> {
	id: number | string;
	factTrackedInMinutes?: number;
}

export interface BillableTimesheet extends Partial<TimesheetRequest> {
	id: number | string;
	billableTrackedInMinutes?: number;
}

export type UseTooltipHandlerProps = Pick<
	TimeSheetContextValues,
	'isEngineer' | 'setTooltipContent' | 'displayTooltips'
> & {
	row: TimeSheetRow;
	column: CalculatedColumn<TimeSheetRow, unknown>;
};
