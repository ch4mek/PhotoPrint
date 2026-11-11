import { Dispatch, SetStateAction } from 'react';

import { defaultHolidaysFiltersValues } from '../constants';
import {
	HolidaysAction,
	HolidaysFilterActions,
	HolidaysFilters,
	HolidaysFiltersHandlers,
} from '../types';

import { FromToProps } from '@/components/Grid';
import { handleSetStateAction } from '@/helpers/handleSetStateAction';

export const generateSetFilters = (
	state: HolidaysFilters,
	dispatch: Dispatch<HolidaysAction>
): HolidaysFiltersHandlers => ({
	dateType: (payloadForAction: SetStateAction<string[]>) => {
		const payload = handleSetStateAction(state.dateType, payloadForAction);

		dispatch({ type: HolidaysFilterActions.HOLIDAY_TYPE, payload });
	},
	countryId: (payloadForAction: SetStateAction<string[]>) => {
		const payload = handleSetStateAction(state.countryId, payloadForAction);

		dispatch({ type: HolidaysFilterActions.COUNTRY_ID, payload });
	},
	name: (payloadForAction: SetStateAction<string>) => {
		const payload = handleSetStateAction(state.name, payloadForAction);

		dispatch({ type: HolidaysFilterActions.HOLIDAY_NAME, payload });
	},
	date: (payloadForAction: SetStateAction<FromToProps<Date>>) => {
		const payload = handleSetStateAction(state.date, payloadForAction);

		dispatch({ type: HolidaysFilterActions.DATE, payload });
	},
	newDate: (payloadForAction: SetStateAction<FromToProps<Date>>) => {
		const payload = handleSetStateAction(state.newDate, payloadForAction);

		dispatch({ type: HolidaysFilterActions.NEW_DATE, payload });
	},
	reset: () =>
		dispatch({
			type: HolidaysFilterActions.RESET,
			payload: defaultHolidaysFiltersValues,
		}),
});
