import { DefaultSortFields, SortDirection } from '@/constants/sort';
import type { UpdateClientsResponse } from '@/queries/types';
import type { EquipmentItemResponse } from '@/types/equipment';
import type { ProjectEntity } from '@/types/project';
import type { UseUpdateUserResponse } from '@/types/userInfo';

const sortUpperCaseFirst =
	(sortBy: string | number) =>
	(a: { [x: string]: number }, b: { [x: string]: number }) => {
		if (sortBy === DefaultSortFields.ID) {
			return a[sortBy] - b[sortBy];
		}

		return a[sortBy] < b[sortBy] ? -1 : 1;
	};

const sortUpperCaseLast =
	(sortBy: string | number) =>
	(a: { [x: string]: number }, b: { [x: string]: number }) => {
		if (sortBy === DefaultSortFields.ID) {
			return b[sortBy] - a[sortBy];
		}

		return a[sortBy] > b[sortBy] ? -1 : 1;
	};

export const updateGridItems = (
	previousState: any,
	updatedItem:
		| ProjectEntity
		| UseUpdateUserResponse
		| UpdateClientsResponse
		| EquipmentItemResponse,
	sortBy: string,
	order?: SortDirection
) => {
	const updatedItemList = previousState?.items
		? previousState.items.map((item: { id: string | number }) =>
				item.id === updatedItem?.id ? updatedItem : item
		  )
		: [];
	const sortFunc =
		order === SortDirection.ASC ? sortUpperCaseFirst : sortUpperCaseLast;

	return {
		...previousState,
		items: updatedItemList.sort(sortFunc(sortBy)),
	};
};
