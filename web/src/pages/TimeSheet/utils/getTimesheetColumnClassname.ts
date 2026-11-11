import { HOVER_COLUMN_CELL_CLASS_PREFIX } from '../components/TimeSheeetManager/constants';

import { getHoverColumnIndexByOrder } from './getHoverColumnIndexByOrder';

export const getTimesheetColumnClassname = (
	index: number,
	isIndexCalculated: boolean = false
) =>
	`${HOVER_COLUMN_CELL_CLASS_PREFIX}-${
		isIndexCalculated ? index : getHoverColumnIndexByOrder(index)
	}`;
