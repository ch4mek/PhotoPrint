import { useMemo } from 'react';

import { TOTAL_BAR_NAME } from '../constants';
import { TooltipPayload } from '../types';

import { TooltipPayloadItem } from './types';

const getFullHoursFromFloatHours = (value: number) => Math.floor(value);

const getRemainMinutesFromFloatHours = (value: number) =>
	Math.round(value * 60) % 60;

export function TotalTooltip(payload: TooltipPayloadItem[]) {
	const { name } = payload[0].payload;

	const sumOfHours = payload.reduce(
		(acc, project) =>
			project.value && project.dataKey !== TOTAL_BAR_NAME
				? acc + project.value
				: acc,
		0
	);

	const isMonthFilter = useMemo(
		() => typeof name === 'object' && name?.date && name?.weekDay,
		[name]
	);

	return {
		content: isMonthFilter ? `${name.weekDay}, ${name.date}` : String(name),
		hours: getFullHoursFromFloatHours(sumOfHours),
		minutes: getRemainMinutesFromFloatHours(sumOfHours),
	};
}

export function projectTooltip(tooltipPayload: TooltipPayload) {
	const projectName = tooltipPayload?.tooltipPayload[0]?.name;

	const value = tooltipPayload?.tooltipPayload[0]?.value;

	return {
		content: projectName,
		hours: getFullHoursFromFloatHours(value),
		minutes: getRemainMinutesFromFloatHours(value),
	};
}
