import { defaultSearchOptions } from '@/constants/search';
import { TransferListItem } from '@/forms/Absence/AbsenceAssignmentForm/types';
import { searchDataByTitle } from '@/forms/Absence/AbsenceAssignmentForm/utils/searchDataByTitle';

export const filterItems = (searchValue: string, items: TransferListItem[]) => {
	if (!searchValue || searchValue.length > defaultSearchOptions.maxCharacters) {
		return items;
	}

	return searchDataByTitle(items, searchValue);
};
