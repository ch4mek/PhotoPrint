import { useLayoutEffect, useState } from 'react';

import {
	UseDevelopmentPlanRowsHookReturn,
	UserDevelopmentPlanRow,
} from '../types';
import { mapDevelopmentPlanResponseToRows } from '../utils';

import { UserDevelopmentPlan } from '@/types/developmentPlan';

export function useDevelopmentPlanRows(
	data?: UserDevelopmentPlan[]
): UseDevelopmentPlanRowsHookReturn {
	const [rows, setRows] = useState<UserDevelopmentPlanRow[]>([]);

	useLayoutEffect(() => {
		if (data) {
			setRows(mapDevelopmentPlanResponseToRows(data));
		}
	}, [data]);

	return [rows, setRows];
}
