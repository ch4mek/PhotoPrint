import {
	DEADLINE_DATE_FIELD_NAME,
	DEFAULT_PLANS_ORDER_BY_FIELD,
	ID_FIELD_NAME,
} from '@/constants/developmentPlan';
import { SortDirection } from '@/constants/sort';
import { capitalizeString } from '@/helpers/capitalizeString';

export const getOrderValueForDevelopmentPlans = (
	orderBy: string,
	order: SortDirection
) => {
	const defaultOrderOption = `${capitalizeString(orderBy)} ${order}`;

	if (orderBy === DEFAULT_PLANS_ORDER_BY_FIELD) {
		return `${defaultOrderOption}, ${capitalizeString(
			DEADLINE_DATE_FIELD_NAME
		)} ${SortDirection.ASC}, 
		${ID_FIELD_NAME} ${SortDirection.DESC}`;
	}

	return defaultOrderOption;
};
