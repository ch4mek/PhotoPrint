import { TOTAL_BAR_NAME } from '../constants';

import { ChartCustomizedTooltip } from '@/components/ChartCustomizedTooltip';

import type { CustomizedTooltipProps } from './types';
import { projectTooltip, TotalTooltip } from './utils';

export function CustomizedTooltip({
	active,
	payload,
	tooltipPayload,
	isTooltipOnLeftSide,
	ref,
}: CustomizedTooltipProps) {
	if (!active || !tooltipPayload || !payload || !payload.length) {
		return null;
	}

	const tooltipDatakey = tooltipPayload?.tooltipPayload[0]?.dataKey;

	const { content, hours, minutes } =
		tooltipDatakey === TOTAL_BAR_NAME
			? TotalTooltip(payload)
			: projectTooltip(tooltipPayload);

	return (
		<ChartCustomizedTooltip
			ref={ref}
			headerText={content}
			mainText={`${hours > 0 ? `${hours} h ` : ''}
				${minutes} min`}
			arrowPlacement={isTooltipOnLeftSide ? 'right' : 'left'}
		/>
	);
}
