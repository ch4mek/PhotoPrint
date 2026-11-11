import isFunction from 'lodash/isFunction';
import { Dispatch, SetStateAction } from 'react';

import { FromToProps } from '@/components/Grid';
import { areArraysEqualByPrimitiveItems } from '@/helpers/areValuesEqual';

import { defaultUserEquipmentFilters } from './constants';
import {
	UserEquipmentFiltersHandlers,
	UserEquipmentFilterActions,
	UserEquipmentFilters,
	UserEquipmentReducerAction,
} from './types';

export function userEquipmentReducer(
	state: UserEquipmentFilters,
	{ type, payload }: UserEquipmentReducerAction
): UserEquipmentFilters {
	switch (type) {
		case UserEquipmentFilterActions.RESET:
			return { ...payload };
		case UserEquipmentFilterActions.EQUIPMENT_NAME:
			return { ...state, equipmentName: payload };
		case UserEquipmentFilterActions.EQUIPMENT_TYPE:
			return { ...state, equipmentType: payload };
		case UserEquipmentFilterActions.SERIAL_NUMBER:
			return { ...state, serialNumber: payload };
		case UserEquipmentFilterActions.INVENTORY_NUMBER:
			return { ...state, inventoryNumber: payload };
		case UserEquipmentFilterActions.IS_ASSIGNED:
			return { ...state, isAssigned: payload };
		case UserEquipmentFilterActions.ISSUE_DATE:
			return {
				...state,
				issueDate: {
					...state.issueDate,
					...payload,
				},
			};
		case UserEquipmentFilterActions.RETURN_DATE:
			return {
				...state,
				returnDate: {
					...state.returnDate,
					...payload,
				},
			};
		default:
			return state;
	}
}

const handleSetStateAction = <T>(
	prevValue: T,
	payload: SetStateAction<T>
): T => {
	if (isFunction(payload)) {
		return payload(prevValue);
	}

	return payload;
};

export const createUserEquipmentFiltersHandlers = (
	state: UserEquipmentFilters,
	dispatch: Dispatch<UserEquipmentReducerAction>
): UserEquipmentFiltersHandlers => ({
	reset: () =>
		dispatch({
			type: UserEquipmentFilterActions.RESET,
			payload: defaultUserEquipmentFilters,
		}),
	equipmentName: (payloadForAction: SetStateAction<string>) => {
		const payload = handleSetStateAction(state.equipmentName, payloadForAction);

		dispatch({ type: UserEquipmentFilterActions.EQUIPMENT_NAME, payload });
	},
	equipmentType: (payloadForAction: SetStateAction<string[]>) => {
		const payload = handleSetStateAction(state.equipmentType, payloadForAction);

		dispatch({ type: UserEquipmentFilterActions.EQUIPMENT_TYPE, payload });
	},
	serialNumber: (payloadForAction: SetStateAction<string>) => {
		const payload = handleSetStateAction(state.serialNumber, payloadForAction);

		dispatch({ type: UserEquipmentFilterActions.SERIAL_NUMBER, payload });
	},
	inventoryNumber: (payloadForAction: SetStateAction<string>) => {
		const payload = handleSetStateAction(
			state.inventoryNumber,
			payloadForAction
		);

		dispatch({ type: UserEquipmentFilterActions.INVENTORY_NUMBER, payload });
	},
	issueDate: (payloadForAction: SetStateAction<FromToProps<Date>>) => {
		const payload = handleSetStateAction(state.issueDate, payloadForAction);

		dispatch({ type: UserEquipmentFilterActions.ISSUE_DATE, payload });
	},
	isAssigned: (payloadForAction: SetStateAction<string[]>) => {
		const payload = handleSetStateAction(state.isAssigned, payloadForAction);

		dispatch({ type: UserEquipmentFilterActions.IS_ASSIGNED, payload });
	},
	returnDate: (payloadForAction: SetStateAction<FromToProps<Date>>) => {
		const payload = handleSetStateAction(state.returnDate, payloadForAction);

		dispatch({ type: UserEquipmentFilterActions.RETURN_DATE, payload });
	},
});

export const isResetDisabled = (filters: UserEquipmentFilters) => {
	const keys = Object.keys(filters) as Array<keyof typeof filters>;

	const isDisabled = keys.every((key) => {
		const currentField = filters[key];
		const defaultField = defaultUserEquipmentFilters[key];

		if (Array.isArray(currentField) && Array.isArray(defaultField)) {
			return areArraysEqualByPrimitiveItems(currentField, defaultField);
		}

		return currentField === defaultField;
	});

	return isDisabled;
};
