import { DAYS_COLUMNS_START_INDEX } from '../constants';

export const getHoverColumnIndexByOrder = (index: number) =>
	index + DAYS_COLUMNS_START_INDEX;
