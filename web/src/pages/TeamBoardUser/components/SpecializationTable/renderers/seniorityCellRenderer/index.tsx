import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import { Skeleton } from '@mui/material';
import { RenderCellProps } from 'react-data-grid';

import { TOOLTIP_FALLBACK_PLACEMENTS } from '@/components/Grid/constants';
import { zIndexes } from '@/constants/zIndexes';
import { SpecializationRow } from '@/pages/TeamBoardUser/types';

import { CellWrapper } from './styles';

export function seniorityCellRenderer(
	{
		row: { showSkeleton, snapshots },
		column: { key },
	}: RenderCellProps<SpecializationRow, unknown>,
	isMarkAsPast?: boolean
) {
	if (showSkeleton) {
		return (
			<CellWrapper>
				<Skeleton animation="wave" width="100%" />
			</CellWrapper>
		);
	}

	const seniority = snapshots[key]?.seniorityLevelName ?? '';

	return (
		<Tooltip
			zIndex={zIndexes.gridTooltip}
			text={seniority}
			position="bottom"
			fallbackPlacements={TOOLTIP_FALLBACK_PLACEMENTS}
			useIfTextCut
		>
			<CellWrapper isMarkAsPast={isMarkAsPast}>
				<Typography variant="m-600">{seniority}</Typography>
			</CellWrapper>
		</Tooltip>
	);
}
