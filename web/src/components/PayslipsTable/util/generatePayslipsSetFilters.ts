import {
	PayslipsAction,
	PayslipsFiltersSetter,
	PayslipsTableFilters,
	PayslipsTableFiltersKeys,
} from '../types';

import { FromToProps } from '@/components/Grid';

export const generatePayslipsSetFilters = (
	dispatch: React.Dispatch<PayslipsAction>
): PayslipsFiltersSetter => ({
	reportDate: (newDate: FromToProps<Date>) =>
		dispatch({ type: PayslipsTableFiltersKeys.reportDate, payload: newDate }),
	set: (newFilters: PayslipsTableFilters) =>
		dispatch({ type: PayslipsTableFiltersKeys.set, payload: newFilters }),
});
