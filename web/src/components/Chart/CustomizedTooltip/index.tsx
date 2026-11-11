import { useMemo } from 'react';

import { ChartCustomizedTooltip } from '@/components/ChartCustomizedTooltip';

import type { CustomizedTooltipProps } from './types';

export function CustomizedTooltip({
	active,
	payload,
	isTooltipOnLeftSide,
	ref,
}: CustomizedTooltipProps) {
	const { name, hours, minutes } = payload?.[0]?.payload ?? {};

	const headerText = useMemo(() => {
		const month = typeof name === 'string' ? name : undefined;
		let result = '';

		if (name?.date) {
			if (name.weekDay) {
				result = `${name.weekDay}, ${name.date}`;
			} else {
				result = name.date;
			}
		} else if (month) {
			result = month;
		}

		return result;
	}, [name]);

	if (!active || !payload || !payload.length) {
		return null;
	}

	return (
		<ChartCustomizedTooltip
			headerText={headerText}
			ref={ref}
			arrowPlacement={isTooltipOnLeftSide ? 'right' : 'left'}
			mainText={`${hours === 0 ? '' : `${hours} h`} ${minutes} min`}
		/>
	);
}
