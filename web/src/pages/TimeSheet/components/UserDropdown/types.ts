import { TimesheetModeFilterOption } from '../../types';
import { TimesheetModeFilterSetter } from '../TimeSheetControlsBar/types';

import { WithMobileView } from '@/types/filter';

export interface UserDropdownProps extends WithMobileView {
	departmentTypeFilter: TimesheetModeFilterOption | null;
	setDepartmentTypeFilter: TimesheetModeFilterSetter;
}

export interface UserOption {
	name: string;
	description: string;
}
