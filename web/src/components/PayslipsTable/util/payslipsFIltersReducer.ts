import {
	PayslipsAction,
	PayslipsTableFilters,
	PayslipsTableFiltersKeys,
} from '../types';

import { FromToProps } from '@/components/Grid';

export const payslipsFiltersReducer = (
	state: PayslipsTableFilters,
	{ type, payload }: PayslipsAction
): PayslipsTableFilters => {
	switch (type) {
		case PayslipsTableFiltersKeys.reportDate: {
			return { ...state, reportDate: payload as FromToProps<Date> };
		}

		case PayslipsTableFiltersKeys.set: {
			return payload as PayslipsTableFilters;
		}

		default: {
			return state;
		}
	}
};
