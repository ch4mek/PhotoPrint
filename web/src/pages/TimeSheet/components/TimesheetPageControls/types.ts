import { TimesheetStatusType } from '../../types';
import {
	TimeSheetControlsBarProps,
	TimesheetModeFilterSetter,
} from '../TimeSheetControlsBar/types';

import { ALL_ID } from '@/constants/filters';

export interface FilterOption<IdType = typeof ALL_ID | number | string> {
	id: IdType;
	name: string;
}

export interface TimesheetPageControlsProps extends TimeSheetControlsBarProps {
	currentDate: Date;
	clearAllFilter: () => void;
	isResetDisabled: boolean;
	setDepartmentTypeFilter: TimesheetModeFilterSetter;
	engineerStatus?: TimesheetStatusType | null;
	shouldRenderSubmitStatusForEmployee: boolean;
	isEmployeeOrSysAdminRole: boolean;
}

export interface FiltersWrapperProps {
	isSubmitRendered: boolean;
}
