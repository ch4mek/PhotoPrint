import { useContext } from 'react';

import { TimesheetParametersContext } from '../context/timesheetParametersContext';
import { isEmployeeOfPMO } from '../utils/isEmployeeOfPMO';

import { useAuth } from '@/context/AuthContext';

export const useIsPersonalModeOfPMO = () => {
	const { role, departmentId } = useAuth();
	const { isPersonalModeOn } = useContext(TimesheetParametersContext);

	return isEmployeeOfPMO(role, departmentId) && Boolean(isPersonalModeOn);
};
