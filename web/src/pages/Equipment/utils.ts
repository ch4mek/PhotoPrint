import { FromToProps } from '@/components/Grid';
import { getFullName } from '@/helpers/utils';
import { ResponseUser } from '@/queries/types';

import {
	EquipmentActionType,
	EquipmentFilters,
	EquipmentReducerType,
	SetFiltersType,
} from './types';

const normalizeUsers = (users: ResponseUser[]) =>
	users.map(({ lastName, firstName }) => getFullName(firstName, lastName));

export const getUsersOptions = (users?: ResponseUser[]) => {
	if (!users) {
		return [];
	}

	return normalizeUsers(users);
};

export const createSetFilters = (
	dispatch: React.Dispatch<EquipmentReducerType>
): SetFiltersType => ({
	id: (payload: string) => dispatch({ type: EquipmentActionType.ID, payload }),
	reset: (payload: EquipmentFilters) =>
		dispatch({ type: EquipmentActionType.RESET, payload }),
	avaliability: (payload?: string[]) =>
		dispatch({ type: EquipmentActionType.AVALIABILITY, payload }),
	storage: (payload?: string[]) =>
		dispatch({ type: EquipmentActionType.STORAGE, payload }),
	serialNumber: (payload: string) =>
		dispatch({ type: EquipmentActionType.SERIAL_NUMBER, payload }),
	inventoryNumber: (payload: string) =>
		dispatch({ type: EquipmentActionType.INVENTORY_NUMBER, payload }),
	invoiceNumber: (payload: string) =>
		dispatch({ type: EquipmentActionType.INVOICE_NUMBER, payload }),
	name: (payload: string) =>
		dispatch({ type: EquipmentActionType.NAME, payload }),
	typeId: (payload?: string[]) =>
		dispatch({ type: EquipmentActionType.TYPE, payload }),
	user: (payload?: string) =>
		dispatch({ type: EquipmentActionType.USER, payload }),
	departments: (payload?: string[]) =>
		dispatch({ type: EquipmentActionType.DEPARTMENTS, payload }),
	approver: (payload?: string) =>
		dispatch({ type: EquipmentActionType.APPROVER, payload }),
	equipmentCharacteristics: (payload: string) =>
		dispatch({ type: EquipmentActionType.EQUIPMENT_CHARACTERISTICS, payload }),
	guaranteeDate: (payload: FromToProps<Date>) =>
		dispatch({ type: EquipmentActionType.GUARANTEE_DATE, payload }),
	location: (payload?: string[]) =>
		dispatch({ type: EquipmentActionType.LOCATION, payload }),
	notes: (payload: string) =>
		dispatch({ type: EquipmentActionType.NOTES, payload }),
	purchasePlace: (payload: string) =>
		dispatch({ type: EquipmentActionType.PURCHASE_PLACE, payload }),
	purchaseDate: (payload: FromToProps<Date>) =>
		dispatch({ type: EquipmentActionType.PURCHASE_DATE, payload }),
	price: (payload: FromToProps<string>) =>
		dispatch({ type: EquipmentActionType.PRICE, payload }),
	repairPlace: (payload: string) =>
		dispatch({ type: EquipmentActionType.REPAIR_PLACE, payload }),
});

export const equipmentReducer = (
	state: EquipmentFilters,
	{ type, payload }: EquipmentReducerType
): EquipmentFilters => {
	switch (type) {
		case EquipmentActionType.RESET: {
			return { ...(payload as EquipmentFilters) };
		}
		case EquipmentActionType.AVALIABILITY: {
			return { ...state, availability: payload as string[] };
		}
		case EquipmentActionType.ID: {
			return { ...state, id: payload as string };
		}
		case EquipmentActionType.SERIAL_NUMBER: {
			return { ...state, serialNumber: payload as string };
		}
		case EquipmentActionType.INVENTORY_NUMBER: {
			return { ...state, inventoryNumber: payload as string };
		}
		case EquipmentActionType.INVOICE_NUMBER: {
			return { ...state, invoiceNumber: payload as string };
		}
		case EquipmentActionType.NAME: {
			return { ...state, name: payload as string };
		}
		case EquipmentActionType.TYPE: {
			return { ...state, equipmentTypeId: payload as string[] };
		}
		case EquipmentActionType.USER: {
			return { ...state, user: payload as string };
		}
		case EquipmentActionType.DEPARTMENTS: {
			return { ...state, departments: payload as string[] };
		}
		case EquipmentActionType.EQUIPMENT_CHARACTERISTICS: {
			return { ...state, equipmentCharacteristics: payload as string };
		}
		case EquipmentActionType.PURCHASE_PLACE: {
			return { ...state, purchasePlace: payload as string };
		}
		case EquipmentActionType.REPAIR_PLACE: {
			return { ...state, repairPlace: payload as string };
		}
		case EquipmentActionType.APPROVER: {
			return { ...state, approver: payload as string };
		}
		case EquipmentActionType.PURCHASE_DATE: {
			return {
				...state,
				purchaseDate: {
					...state.purchaseDate,
					...(payload as FromToProps<Date>),
				},
			};
		}
		case EquipmentActionType.GUARANTEE_DATE: {
			return {
				...state,
				guaranteeDate: {
					...state.purchaseDate,
					...(payload as FromToProps<Date>),
				},
			};
		}
		case EquipmentActionType.LOCATION: {
			return { ...state, location: payload as string[] };
		}
		case EquipmentActionType.STORAGE: {
			return { ...state, storage: payload as string[] };
		}
		case EquipmentActionType.NOTES: {
			return { ...state, notes: payload as string };
		}
		case EquipmentActionType.PRICE: {
			return {
				...state,
				price: {
					...state.price,
					...(payload as FromToProps<string>),
				} as FromToProps<string>,
			};
		}

		default:
			return state;
	}
};
