import { TimesheetModeFilterOption } from '../../types';
import { TimesheetModeFilterSetter } from '../TimeSheetControlsBar/types';

import { WithMobileView } from '@/types/filter';

export interface RoleSelectWithDepartmentsProps extends WithMobileView {
	departmentTypeFilter: TimesheetModeFilterOption | null;
	setDepartmentTypeFilter: TimesheetModeFilterSetter;
}
