import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { Skeleton } from '@mui/material';

import { TOOLTIP_FALLBACK_PLACEMENTS } from '@/components/Grid/constants';
import { zIndexes } from '@/constants/zIndexes';

import { CellTypography, CellWrapper } from './styles';
import { HardSkillCellProps } from './types';

export function HardSkillCell({
	content,
	isPrimary,
	showSkeleton = false,
}: HardSkillCellProps) {
	if (showSkeleton) {
		return (
			<CellWrapper isPrimary={false}>
				<Skeleton animation="wave" width="100%" />
			</CellWrapper>
		);
	}

	return (
		<Tooltip
			zIndex={zIndexes.gridTooltip}
			text={content}
			position="bottom"
			fallbackPlacements={TOOLTIP_FALLBACK_PLACEMENTS}
			useIfTextCut
		>
			<CellWrapper isPrimary={isPrimary}>
				<CellTypography variant="m-400" isPrimary={isPrimary}>
					{content}
				</CellTypography>
			</CellWrapper>
		</Tooltip>
	);
}
