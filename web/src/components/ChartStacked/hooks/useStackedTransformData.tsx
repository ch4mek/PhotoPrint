import { useTheme } from '@emotion/react';
import { useMemo } from 'react';

import {
	TOTAL_BAR_NAME,
	Y_AXIS_DOMAIN_MAX_BASE_SIZE,
	Y_AXIS_TICKS_COUNT,
	Y_AXIS_TICKS_STEP,
} from '../constants';
import type { ChartStackedProjectAdapted } from '../types';
import {
	calculateMaxYAxisValueInHours,
	createKeyWithNameAndColorMap,
} from '../utils';

export function useStackedTransformData(data: ChartStackedProjectAdapted[]) {
	const theme = useTheme();

	const calculatedYAxisTicks = useMemo(() => {
		const ticksMaxValueInMinutes = calculateMaxYAxisValueInHours(data);

		const ticksStep = Math.ceil(ticksMaxValueInMinutes / Y_AXIS_TICKS_STEP);

		return Array.from({ length: Y_AXIS_TICKS_COUNT }, (_, i) => i * ticksStep);
	}, [data]);

	const keyWithNameAndColorMap = useMemo(
		() =>
			createKeyWithNameAndColorMap(
				data,
				calculatedYAxisTicks.at(-1),
				theme.palette['primary-2']
			),
		[data, calculatedYAxisTicks, theme]
	);

	const transformedData = useMemo(
		() =>
			data.map((item) => ({
				name: item.name,
				lastProjectIndex:
					item.projects[item.projects.length - 2]?.projectId || null,
				...item.projects.reduce(
					(acc, cur) => ({
						...acc,
						[cur.projectId]:
							cur.projectId === TOTAL_BAR_NAME
								? Math.max(cur.hoursForBar, Y_AXIS_DOMAIN_MAX_BASE_SIZE)
								: cur.hoursForBar,
					}),
					{}
				),
			})),
		[data]
	);

	return {
		keyWithNameAndColorMap,
		transformedData,
		calculatedYAxisTicks,
	};
}
