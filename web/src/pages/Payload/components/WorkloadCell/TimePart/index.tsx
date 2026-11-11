import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { useMemo } from 'react';

import { PlanCompletionStatus } from '../constants';

import { TOOLTIP_FALLBACK_PLACEMENTS } from '@/components/Grid/constants';
import { zIndexes } from '@/constants/zIndexes';
import { getPlanCompletion } from '@/pages/Payload/utils/getPlanCompletion';

import {
	FirstWorkloadPart,
	SecondWorkloadPart,
	TimePartWrapper,
} from './styles';
import { TimePartProps, TypeOfTimePart } from './types';

export function TimePart({
	type,
	firstTime,
	secondTime,
	isCurrentDate = false,
	isFutureDate = false,
	isSubRow = false,
	disabled = false,
}: TimePartProps) {
	const planCompletion = useMemo<PlanCompletionStatus>(
		() => getPlanCompletion(firstTime, secondTime),
		[firstTime, secondTime]
	);

	const isShowFirstValue =
		type === TypeOfTimePart.Total ||
		isCurrentDate ||
		(firstTime !== secondTime && !isFutureDate);

	return (
		<TimePartWrapper {...{ type, disabled }}>
			{isShowFirstValue && (
				<Tooltip
					zIndex={zIndexes.gridTooltip}
					text={String(firstTime)}
					position="bottom"
					fallbackPlacements={TOOLTIP_FALLBACK_PLACEMENTS}
					useIfTextCut
				>
					<FirstWorkloadPart {...{ type, isSubRow, planCompletion }}>
						{firstTime}
					</FirstWorkloadPart>
				</Tooltip>
			)}
			<Tooltip
				zIndex={zIndexes.gridTooltip}
				text={String(secondTime)}
				position="bottom"
				fallbackPlacements={TOOLTIP_FALLBACK_PLACEMENTS}
				useIfTextCut
			>
				<SecondWorkloadPart
					{...{ type, isSubRow, planCompletion }}
					isPrimaryValue={!isShowFirstValue}
				>
					{secondTime}
				</SecondWorkloadPart>
			</Tooltip>
		</TimePartWrapper>
	);
}
