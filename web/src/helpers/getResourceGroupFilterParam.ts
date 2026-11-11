import { extraFiltersBy, NO_VALUE_FILTER_OPTION } from '@/constants/filters';
import { Filter } from '@/types/filter';

const { All } = extraFiltersBy;
const WITHOUT_GROUP_FILTER_PARAM = 0;

export const getResourceGroupFilterParam = (
	groupFilter?: Filter<string | number> | null
) => {
	if (!groupFilter || groupFilter?.id === All) {
		return undefined;
	}

	return groupFilter.id !== NO_VALUE_FILTER_OPTION.id
		? groupFilter.id
		: WITHOUT_GROUP_FILTER_PARAM;
};
