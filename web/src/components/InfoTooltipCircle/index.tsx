import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';

import { TOOLTIP_FALLBACK_PLACEMENTS } from '../Grid/constants';

import { InfoCircle, InfoCircleIcon } from './styles';
import { InfoTooltipCircleProps } from './types';

export function InfoTooltipCircle({
	tooltipContent,
	isWithFallback,
	stylesConfig,
}: InfoTooltipCircleProps) {
	return (
		<Tooltip
			text={tooltipContent}
			fallbackPlacements={
				isWithFallback ? TOOLTIP_FALLBACK_PLACEMENTS : undefined
			}
			position="right"
		>
			<InfoCircle {...{ stylesConfig }}>
				<InfoCircleIcon />
			</InfoCircle>
		</Tooltip>
	);
}
